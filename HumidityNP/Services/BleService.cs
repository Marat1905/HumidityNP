using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using HumidityNP.Models;
using HumidityNP.Extensions;

namespace HumidityNP.Services
{
    public class BleService : IBleService
    {
        private readonly IAdapter _adapter;
        private readonly IBluetoothLE _bluetoothLe;
        private IDevice _connectedDevice;
        private ICharacteristic _readingCharacteristic;
        private CancellationTokenSource _scanCts;
        private bool _isSearching;      // true, когда выполняется поиск (сканирование)
        private bool _isConnecting;     // true, когда идёт подключение к конкретному устройству
        private bool _autoReconnect = true;
        private bool _disposed;
        private ParsedHumidityData? _lastData;
        private readonly object _lock = new();
        private readonly Dictionary<Guid, string> _deviceNameCache = new();

        // UUID характеристики New Reading (X0 Series Protocol)
        private static readonly Guid ReadingCharacteristicUuid =
            Guid.Parse("630a0404-0852-11e9-8f0b-0080d1c0f75b");

        // Префиксы имён устройств Delmhorst X0 Series
        private readonly string[] _deviceNamePrefixes =
            { "BDX", "JX", "FX", "CX", "PX", "JLX", "HTX" };

        public event Action<ParsedHumidityData> OnDataReceived;
        public event Action<string> OnStatusChanged;

        public bool IsConnected => _connectedDevice?.State == DeviceState.Connected;
        public ParsedHumidityData? LastData => _lastData;
        public bool IsConnecting => _isConnecting;

        public BleService()
        {
            _bluetoothLe = CrossBluetoothLE.Current;
            _adapter = CrossBluetoothLE.Current.Adapter;

            _adapter.DeviceDiscovered += OnDeviceDiscovered;
            _adapter.DeviceConnected += OnDeviceConnected;
            _adapter.DeviceDisconnected += OnDeviceDisconnected;
            _adapter.DeviceConnectionLost += OnDeviceConnectionLost;
        }

        /// <summary>
        /// Запускает непрерывное сканирование до тех пор, пока не будет найдено подходящее устройство
        /// и не установлено соединение. При обнаружении устройства сразу пытается подключиться.
        /// </summary>
        public async Task StartAutoConnectAsync()
        {
            lock (_lock)
            {
                if (_isSearching || IsConnected) return;
                _isSearching = true;
                _autoReconnect = true;
            }

            try
            {
                while (_autoReconnect && !IsConnected && !_disposed)
                {
                    OnStatusChanged?.Invoke("Поиск устройства...");

                    // Запускаем сканирование без ограничения по времени
                    _scanCts = new CancellationTokenSource();

                    try
                    {
                        await _adapter.StartScanningForDevicesAsync(
                            cancellationToken: _scanCts.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        // Сканирование отменено (успешное подключение или Disconnect)
                    }
                    catch (Exception ex)
                    {
                        OnStatusChanged?.Invoke($"Ошибка сканирования: {ex.Message}");
                    }

                    // Если вышли из цикла сканирования без подключения, ждём перед повтором
                    if (!IsConnected && _autoReconnect && !_disposed)
                    {
                        OnStatusChanged?.Invoke("Повтор поиска через 3 секунды...");
                        await Task.Delay(3000);
                    }
                }
            }
            finally
            {
                lock (_lock)
                {
                    _isSearching = false;
                }
            }
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            var device = e.Device;
            string deviceName = device.Name;
            //Console.WriteLine($"Имя = {device.Name}; UUID = {device.Id}");

            // Если имя пустое, пытаемся получить из кэша
            if (string.IsNullOrEmpty(deviceName))
            {
                if (_deviceNameCache.TryGetValue(device.Id, out string cachedName))
                {
                    deviceName = cachedName;
                }
                else
                {
                    // Если в кэше нет, то игнорируем, так как не знаем, наше ли это устройство
                    return;
                }
            }
            else
            {
                // Сохраняем имя в кэш
                _deviceNameCache[device.Id] = deviceName;
            }

            // Проверяем префикс
            if (_deviceNamePrefixes.Any(prefix => deviceName.StartsWith(prefix)))
            {
                if (_isSearching && !IsConnected && !_isConnecting)
                {
                    OnStatusChanged?.Invoke($"Найдено устройство: {deviceName}, подключение...");
                    _scanCts?.Cancel(); // Останавливаем сканирование
                    _ = ConnectToDeviceAsync(device);
                }
            }
        }

        private async Task ConnectToDeviceAsync(IDevice device)
        {
            // Блокируем параллельные попытки подключения
            lock (_lock)
            {
                if (_isConnecting) return;
                _isConnecting = true;
            }

            try
            {
                var connectCts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                await _adapter.ConnectToDeviceAsync(device, cancellationToken: connectCts.Token);
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke($"Ошибка подключения: {ex.Message}");
                // Если не удалось подключиться, возобновляем поиск (если он ещё не запущен)
                if (!_isSearching && _autoReconnect && !IsConnected)
                {
                    _ = StartAutoConnectAsync();
                }
            }
            finally
            {
                lock (_lock)
                {
                    _isConnecting = false;
                }
            }
        }

        private async void OnDeviceConnected(object sender, DeviceEventArgs e)
        {
            _connectedDevice = e.Device;
            OnStatusChanged?.Invoke("Подключено");

            // Останавливаем сканирование, если оно всё ещё активно
            _scanCts?.Cancel();

            try
            {
                await SubscribeToReadingsAsync();
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke($"Ошибка подписки: {ex.Message}");
            }
        }

        private async Task SubscribeToReadingsAsync()
        {
            if (_connectedDevice == null) return;

            var services = await _connectedDevice.GetServicesAsync();
            foreach (var service in services)
            {
                var characteristics = await service.GetCharacteristicsAsync();
                foreach (var characteristic in characteristics)
                {
                    if (characteristic.Id == ReadingCharacteristicUuid)
                    {
                        _readingCharacteristic = characteristic;

                        var props = characteristic.Properties;
                        if ((props & CharacteristicPropertyType.Indicate) != 0 ||
                            (props & CharacteristicPropertyType.Notify) != 0)
                        {
                            await characteristic.StartUpdatesAsync();
                            characteristic.ValueUpdated += OnCharacteristicValueUpdated;
                            OnStatusChanged?.Invoke("Подписка активна");
                        }
                        else
                        {
                            OnStatusChanged?.Invoke("Характеристика не поддерживает уведомления");
                        }
                        return;
                    }
                }
            }

            OnStatusChanged?.Invoke("Характеристика не найдена");
        }

        private void OnCharacteristicValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            try
            {
                if (e.Characteristic.Value != null && e.Characteristic.Value.Length >= 6)
                {
                    var parsedData = HumidityParser.Parse(e.Characteristic.Value);
                    _lastData = parsedData;
                    OnDataReceived?.Invoke(parsedData);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка парсинга: {ex.Message}");
            }
        }

        private async void OnDeviceDisconnected(object sender, DeviceEventArgs e)
        {
            if (_connectedDevice?.Id == e.Device.Id)
            {
                OnStatusChanged?.Invoke("Соединение потеряно");
                await CleanupConnectionAsync();

                if (_autoReconnect && !_disposed)
                {
                    // Запускаем переподключение
                    _ = StartAutoConnectAsync();
                }
            }
        }

        private async void OnDeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            if (_connectedDevice?.Id == e.Device.Id)
            {
                OnStatusChanged?.Invoke("Соединение потеряно");
                await CleanupConnectionAsync();

                if (_autoReconnect && !_disposed)
                {
                    _ = StartAutoConnectAsync();
                }
            }
        }

        public async Task DisconnectAsync()
        {
            lock (_lock)
            {
                _autoReconnect = false;
                _isSearching = false; // Останавливаем поиск
            }

            _scanCts?.Cancel();

            if (_connectedDevice != null)
            {
                try
                {
                    await _adapter.DisconnectDeviceAsync(_connectedDevice);
                }
                catch { }
            }

            await CleanupConnectionAsync();
            OnStatusChanged?.Invoke("Отключено");
            // Не сбрасываем _lastData, чтобы при следующем подключении показывать последние известные данные
        }

        private async Task CleanupConnectionAsync()
        {
            if (_readingCharacteristic != null)
            {
                _readingCharacteristic.ValueUpdated -= OnCharacteristicValueUpdated;

                try
                {
                    await _readingCharacteristic.StopUpdatesAsync();
                }
                catch { }

                _readingCharacteristic = null;
            }

            _connectedDevice = null;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _adapter.DeviceDiscovered -= OnDeviceDiscovered;
            _adapter.DeviceConnected -= OnDeviceConnected;
            _adapter.DeviceDisconnected -= OnDeviceDisconnected;
            _adapter.DeviceConnectionLost -= OnDeviceConnectionLost;

            _scanCts?.Cancel();
            _scanCts?.Dispose();

            _autoReconnect = false;
            _isSearching = false;
            _isConnecting = false;
            _deviceNameCache.Clear();
        }
    }
}