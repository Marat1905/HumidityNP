using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumidityNP.Enums;
using HumidityNP.Models;
using HumidityNP.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HumidityNP.ViewModels;

public partial class VehicleMeasurementsViewModel : ObservableObject, IDisposable
{
    private readonly IBleService _bleService;
    private readonly ILocalStorageService _localStorage;
    private string _vehicleId;

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
    private DateTime? _lastDataTime;

    [ObservableProperty]
    private bool _isRefreshing;

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

        _bleService.OnStatusChanged += OnStatusChanged;
        _bleService.OnDataReceived += OnDataReceived;

        // Автоматическое подключение при создании VM
        _ = InitializeBleAsync();
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
            LastDataTime = DateTime.Now;
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
            LastDataTime = DateTime.Now;
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
            Source = Enums.MeasurementSource.Auto,
            Timestamp = DateTime.Now
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
                Source = Enums.MeasurementSource.Manual,
                Timestamp = DateTime.Now
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
        _bleService.OnStatusChanged -= OnStatusChanged;
        _bleService.OnDataReceived -= OnDataReceived;
    }
}