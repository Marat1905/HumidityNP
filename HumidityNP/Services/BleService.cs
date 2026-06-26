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

namespace HumidityNP.Services
{
    public class BleService : IBleService
    {
        private readonly IAdapter _adapter;
        private readonly IBluetoothLE _bluetoothLe;
        private IDevice _connectedDevice;
        private ICharacteristic _readingCharacteristic;
        private CancellationTokenSource _scanCts;
        private bool _isConnecting;
        private bool _autoReconnect = true;
        private bool _disposed;

        // UUID характеристики New Reading (X0 Series Protocol)
        private static readonly Guid ReadingCharacteristicUuid =
            Guid.Parse("630a0404-0852-11e9-8f0b-0080d1c0f75b");

        // Префиксы имён устройств Delmhorst X0 Series
        private readonly string[] _deviceNamePrefixes =
            { "BDX", "JX", "FX", "CX", "PX", "JLX", "HTX" };

        public event Action<ParsedHumidityData> OnDataReceived;
        public event Action<string> OnStatusChanged;

        public bool IsConnected => _connectedDevice?.State == DeviceState.Connected;

        public BleService()
        {
            _bluetoothLe = CrossBluetoothLE.Current;
            _adapter = CrossBluetoothLE.Current.Adapter;

            _adapter.DeviceDiscovered += OnDeviceDiscovered;
            _adapter.DeviceConnected += OnDeviceConnected;
            _adapter.DeviceDisconnected += OnDeviceDisconnected;
            _adapter.DeviceConnectionLost += OnDeviceConnectionLost;
        }

        private readonly List<IDevice> _discoveredDevices = new();

        public async Task StartAutoConnectAsync()
        {
            if (_isConnecting) return;
            _isConnecting = true;

            try
            {
                while (_autoReconnect && !IsConnected && !_disposed)
                {
                    OnStatusChanged?.Invoke("Поиск устройства...");

                    _scanCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    _discoveredDevices.Clear();

                    try
                    {
                        await _adapter.StartScanningForDevicesAsync(
                            cancellationToken: _scanCts.Token);
                    }
                    catch (TaskCanceledException) { }

                    if (_discoveredDevices.Any())
                    {
                        var nearestDevice = _discoveredDevices
                            .OrderByDescending(d => d.Rssi)
                            .First();

                        OnStatusChanged?.Invoke($"Подключение к {nearestDevice.Name}...");
                        await ConnectToDeviceAsync(nearestDevice);
                    }
                    else
                    {
                        OnStatusChanged?.Invoke("Устройства не найдены, повтор через 3 сек...");
                        await Task.Delay(3000);
                    }
                }
            }
            finally
            {
                _isConnecting = false;
            }
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            var device = e.Device;
            if (device.Name != null &&
                _deviceNamePrefixes.Any(prefix => device.Name.StartsWith(prefix)))
            {
                if (!_discoveredDevices.Any(d => d.Id == device.Id))
                {
                    _discoveredDevices.Add(device);
                }
            }
        }

        private async Task ConnectToDeviceAsync(IDevice device)
        {
            try
            {
                var connectCts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                await _adapter.ConnectToDeviceAsync(device, cancellationToken: connectCts.Token);
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke($"Ошибка подключения: {ex.Message}");
            }
        }

        private async void OnDeviceConnected(object sender, DeviceEventArgs e)
        {
            _connectedDevice = e.Device;
            OnStatusChanged?.Invoke("Подключено");

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
            _autoReconnect = false;
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
        }
    }
}