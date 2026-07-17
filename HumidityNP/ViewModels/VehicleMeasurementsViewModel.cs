using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumidityNP.Enums;
using HumidityNP.Models;
using HumidityNP.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HumidityNP.ViewModels;

public partial class VehicleMeasurementsViewModel : ObservableObject, IDisposable
{
    private readonly IBleService _bleService;
    private readonly ILocalStorageService _localStorage;
    private string _vehicleId;

    // Флаг, чтобы не подписываться повторно, если уже подписаны
    private bool _isSubscribedToBle;

    // Флаг, что ViewModel была окончательно уничтожена (Dispose)
    private bool _isDisposed;

    [ObservableProperty]
    private Vehicle _vehicle;

    [ObservableProperty]
    private ObservableCollection<HumidityMeasurement> _measurements = new();

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private string _connectionStatus = "Отключено";

    [ObservableProperty]
    private bool _isDataAvailable;

    [ObservableProperty]
    private string _currentHumidity = "-";

    [ObservableProperty]
    private string _currentTemperature = "-";

    [ObservableProperty]
    private string _currentMaterial = "-";

    [ObservableProperty]
    private string _currentMeasurementType = "-";

    [ObservableProperty]
    private string _currentSign = "";

    [ObservableProperty]
    private DateTimeOffset? _lastDataTime;

    [ObservableProperty]
    private bool _isRefreshing;

    /// <summary>
    /// Доступность кнопки "Зафиксировать" – только когда есть подключение и данные.
    /// </summary>
    [ObservableProperty]
    private bool _canCapture;

    public string VehicleId
    {
        get => _vehicleId;
        set
        {
            if (_vehicleId != value)
            {
                _vehicleId = value;
                LoadDataAsync().ConfigureAwait(false);
            }
        }
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddManualMeasurementCommand { get; }
    public ICommand DeleteMeasurementCommand { get; }
    public ICommand ConnectCommand { get; }
    public ICommand CaptureReadingCommand { get; }

    public VehicleMeasurementsViewModel(IBleService bleService, ILocalStorageService localStorage)
    {
        _bleService = bleService;
        _localStorage = localStorage;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddManualMeasurementCommand = new AsyncRelayCommand(AddManualMeasurementAsync);
        DeleteMeasurementCommand = new AsyncRelayCommand<HumidityMeasurement>(DeleteMeasurementAsync);
        ConnectCommand = new AsyncRelayCommand(ConnectAsync);
        CaptureReadingCommand = new AsyncRelayCommand(CaptureReadingAsync);

        // Подписку на события BLE вынесли из конструктора в отдельный метод,
        // чтобы можно было подписываться/отписываться при появлении/исчезновении страницы.
        // Подписка будет выполнена в OnAppearing() страницы.
        // Автоматическое подключение при создании VM (если ещё не подключены)
        _ = InitializeBleAsync();
    }

    /// <summary>
    /// Подписка на события BLE-сервиса.
    /// Вызывается при появлении страницы (OnAppearing).
    /// Повторный вызов безопасен — внутри есть защита от двойной подписки.
    /// </summary>
    public void SubscribeToBleEvents()
    {
        if (_isDisposed || _isSubscribedToBle) return;

        _bleService.OnStatusChanged += OnStatusChanged;
        _bleService.OnDataReceived += OnDataReceived;
        _isSubscribedToBle = true;

        // Сразу синхронизируем текущее состояние подключения,
        // чтобы UI отражал актуальный статус после возврата на страницу.
        SyncConnectionState();
    }

    /// <summary>
    /// Отписка от событий BLE-сервиса.
    /// Вызывается при исчезновении страницы (OnDisappearing).
    /// Не уничтожает ViewModel, только прекращает получение обновлений.
    /// </summary>
    public void UnsubscribeFromBleEvents()
    {
        if (!_isSubscribedToBle) return;

        _bleService.OnStatusChanged -= OnStatusChanged;
        _bleService.OnDataReceived -= OnDataReceived;
        _isSubscribedToBle = false;
    }

    /// <summary>
    /// Синхронизирует UI с текущим состоянием BLE-сервиса.
    /// Нужно, чтобы при возврате на страницу сразу отображался актуальный статус
    /// и последние известные данные, а не «висело» старое состояние.
    /// </summary>
    private void SyncConnectionState()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsConnected = _bleService.IsConnected;

            if (_bleService.IsConnected)
            {
                ConnectionStatus = "Подключено";
                UpdateUiFromLastData();
            }
            else if (_bleService.IsConnecting)
            {
                ConnectionStatus = "Подключение...";
            }
            else
            {
                ConnectionStatus = "Отключено";
            }
        });
    }

    private void OnStatusChanged(string status)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ConnectionStatus = status;
            IsConnected = _bleService.IsConnected;
            if (IsConnected)
            {
                // Если пришёл статус "Подключено", показываем последние данные
                UpdateUiFromLastData();
            }
        });
    }

    private void OnDataReceived(ParsedHumidityData data)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Обновляем отображаемые данные
            CurrentHumidity = data.Value.ToString("F1") + "%";
            CurrentTemperature = data.TemperatureC.ToString("F1") + "°C";
            CurrentMaterial = data.Material.ToString();
            CurrentMeasurementType = data.Type.ToString();
            CurrentSign = data.Sign == SignType.Less ? "<" : data.Sign == SignType.Greater ? ">" : "";

            LastDataTime = DateTimeOffset.Now;
            IsDataAvailable = true;
        });
    }

    private void UpdateUiFromLastData()
    {
        var data = _bleService.LastData;
        if (data != null)
        {
            CurrentHumidity = data.Value.ToString("F1") + "%";
            CurrentTemperature = data.TemperatureC.ToString("F1") + "°C";
            CurrentMaterial = data.Material.ToString();
            CurrentMeasurementType = data.Type.ToString();
            CurrentSign = data.Sign == SignType.Less ? "<" : data.Sign == SignType.Greater ? ">" : "";

            LastDataTime = DateTimeOffset.Now;
            IsDataAvailable = true;
        }
    }

    private async Task InitializeBleAsync()
    {
        if (_bleService.IsConnected)
        {
            // Уже подключены – показываем последние данные
            UpdateUiFromLastData();
            ConnectionStatus = "Подключено";
            IsConnected = true;
        }
        else if (!_bleService.IsConnecting)
        {
            // Не подключены и не идёт попытка – запускаем авто-подключение
            await _bleService.StartAutoConnectAsync();
        }
        // Если IsConnecting == true, ничего не делаем – ждём событий
    }

    private async Task CaptureReadingAsync()
    {
        if (!IsDataAvailable)
        {
            await Shell.Current.DisplayAlert("Нет данных", "Нет актуальных данных с датчика", "OK");
            return;
        }

        if (string.IsNullOrEmpty(VehicleId))
        {
            await Shell.Current.DisplayAlert("Ошибка", "Не выбрана машина", "OK");
            return;
        }

        // Парсим значения из текущих строк
        double humidity;
        double temperature;
        if (!double.TryParse(CurrentHumidity.Replace("%", ""), out humidity))
        {
            await Shell.Current.DisplayAlert("Ошибка", "Не удалось распознать влажность", "OK");
            return;
        }

        if (!double.TryParse(CurrentTemperature.Replace("°C", ""), out temperature))
        {
            // Если не получается, ставим 0
            temperature = 0;
        }

        var measurement = new HumidityMeasurement
        {
            VehicleId = VehicleId,
            HumidityValue = humidity,
            TemperatureC = temperature,
            MeasurementType = CurrentMeasurementType,
            Material = CurrentMaterial,
            Sign = CurrentSign,
            Source = MeasurementSource.Auto,
            Timestamp = DateTimeOffset.Now
        };

        await _localStorage.SaveMeasurementAsync(measurement);
        Measurements.Insert(0, measurement);

        await Shell.Current.DisplayAlert("Успех", "Измерение сохранено", "OK");
    }

    private async Task LoadDataAsync()
    {
        if (string.IsNullOrEmpty(VehicleId)) return;

        IsRefreshing = true;
        try
        {
            // Информация о машине
            var cached = await _localStorage.GetCachedVehiclesAsync();
            Vehicle = cached.FirstOrDefault(v => v.Id == VehicleId);

            // Замеры
            var measurements = await _localStorage.GetMeasurementsByVehicleAsync(VehicleId);

            Measurements.Clear();

            foreach (var m in measurements.OrderByDescending(m => m.Timestamp))
                Measurements.Add(m);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async Task AddManualMeasurementAsync()
    {
        string input = await Shell.Current.DisplayPromptAsync("Ручной замер", "Введите влажность (%):", "OK", "Отмена", "0.0", -1, Keyboard.Numeric);
        if (!string.IsNullOrEmpty(input) && double.TryParse(input, out double value))
        {
            var measurement = new HumidityMeasurement
            {
                VehicleId = VehicleId,
                HumidityValue = value,
                TemperatureC = 0,
                MeasurementType = "Manual",
                Material = "Manual",
                Sign = "None",
                Source = MeasurementSource.Manual,
                Timestamp = DateTimeOffset.Now
            };

            await _localStorage.SaveMeasurementAsync(measurement);
            Measurements.Insert(0, measurement);
        }
    }

    private async Task DeleteMeasurementAsync(HumidityMeasurement measurement)
    {
        if (measurement == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Удаление", $"Удалить замер от {measurement.Timestamp}?", "Да", "Нет");

        if (confirm)
        {
            await _localStorage.DeleteMeasurementAsync(measurement.LocalId);
            Measurements.Remove(measurement);
        }
    }

    private async Task ConnectAsync()
    {
        if (IsConnected)
            await _bleService.DisconnectAsync();
        else
            _ = _bleService.StartAutoConnectAsync();
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        // При полном уничтожении — отписываемся от событий BLE
        UnsubscribeFromBleEvents();
    }

    // Реакция на изменение IsConnected
    partial void OnIsConnectedChanged(bool value)
    {
        UpdateCanCapture();
    }

    // Реакция на изменение IsDataAvailable
    partial void OnIsDataAvailableChanged(bool value)
    {
        UpdateCanCapture();
    }

    /// <summary>
    /// Обновляет доступность кнопки фиксации.
    /// </summary>
    private void UpdateCanCapture()
    {
        CanCapture = IsConnected && IsDataAvailable;
    }
}