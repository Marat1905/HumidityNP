using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumidityNP.Models;
using HumidityNP.Services;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace HumidityNP.ViewModels;

/// <summary>
/// ViewModel для страницы отображения всех локальных замеров.
/// Загружает замеры из локального хранилища, дополняет их информацией о машинах из кеша.
/// Позволяет выгружать все замеры на сервер и удалять отдельные записи.
/// </summary>
public partial class AllMeasurementsViewModel : ObservableObject
{
    private readonly ILocalStorageService _localStorage;
    private readonly IApiService _apiService;

    /// <summary>Коллекция отображаемых элементов (замер + информация о машине).</summary>
    [ObservableProperty]
    private ObservableCollection<MeasurementDisplayItem> _measurements = new();

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _isBusy;

    public ICommand RefreshCommand { get; }
    public ICommand DeleteMeasurementCommand { get; }
    public ICommand UploadAllCommand { get; }

    public AllMeasurementsViewModel(ILocalStorageService localStorage, IApiService apiService)
    {
        _localStorage = localStorage;
        _apiService = apiService;

        RefreshCommand = new AsyncRelayCommand(LoadMeasurementsAsync);
        DeleteMeasurementCommand = new AsyncRelayCommand<MeasurementDisplayItem>(DeleteMeasurementAsync);
        UploadAllCommand = new AsyncRelayCommand(UploadAllAsync);

        LoadMeasurementsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Загружает все замеры и кешированные машины, строит отображаемую коллекцию.
    /// </summary>
    private async Task LoadMeasurementsAsync()
    {
        IsRefreshing = true;
        try
        {
            // Получаем все замеры
            var all = await _localStorage.GetAllMeasurementsAsync();

            // Загружаем кеш машин для получения номера и госномера
            var vehicles = await _localStorage.GetCachedVehiclesAsync();
            var vehicleDict = vehicles.ToDictionary(v => v.Id, v => v);

            Measurements.Clear();

            foreach (var m in all.OrderByDescending(m => m.Timestamp))
            {
                Vehicle vehicle = null;
                if (!string.IsNullOrEmpty(m.VehicleId) && vehicleDict.TryGetValue(m.VehicleId, out var v))
                {
                    vehicle = v;
                }

                string vehicleInfo;
                string vehicleNumber = string.Empty;
                string vehiclePlate = string.Empty;

                if (vehicle != null)
                {
                    vehicleNumber = vehicle.Number;
                    vehiclePlate = vehicle.VehiclePlate?.Replace(" ", "").ToUpperInvariant() ?? string.Empty;
                    vehicleInfo = $"{vehicleNumber} ({vehiclePlate})";
                }
                else
                {
                    // Если машина не найдена в кеше, показываем VehicleId
                    vehicleInfo = m.VehicleId;
                }

                var item = new MeasurementDisplayItem
                {
                    Measurement = m,
                    DisplayVehicleInfo = vehicleInfo,
                    VehicleNumber = vehicleNumber,
                    VehiclePlate = vehiclePlate
                };

                Measurements.Add(item);
            }
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

    /// <summary>
    /// Удаление замера с подтверждением.
    /// </summary>
    private async Task DeleteMeasurementAsync(MeasurementDisplayItem item)
    {
        if (item == null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Удаление",
            $"Удалить замер от {item.Measurement.Timestamp.ToLocalTime():dd.MM.yyyy HH:mm:ss}?",
            "Да", "Нет");

        if (confirm)
        {
            await _localStorage.DeleteMeasurementAsync(item.Measurement.LocalId);
            Measurements.Remove(item);
        }
    }

    /// <summary>
    /// Выгрузка всех замеров на сервер.
    /// После успешной выгрузки успешные записи удаляются из локальной БД и из UI.
    /// </summary>
    private async Task UploadAllAsync()
    {
        var toUpload = Measurements.Select(x => x.Measurement).ToList();
        if (!toUpload.Any())
        {
            await Shell.Current.DisplayAlert("Выгрузка", "Нет замеров для выгрузки", "OK");
            return;
        }

        bool confirm = await Shell.Current.DisplayAlert(
            "Выгрузка",
            $"Выгрузить {toUpload.Count} замеров на сервер?",
            "Да", "Нет");

        if (!confirm) return;

        IsBusy = true;
        try
        {
            var result = await _apiService.UploadMeasurementsAsync(toUpload);

            if (result != null)
            {
                var message = new StringBuilder();
                message.AppendLine($"✅ Успешно создано: {result.CreatedCount}");

                if (result.SkippedCount > 0)
                {
                    message.AppendLine($"⚠️ Пропущено: {result.SkippedCount}");
                }

                // Определяем индексы замеров, которые завершились ошибкой
                var failedIndexes = new HashSet<int>();
                foreach (var error in result.Errors)
                {
                    if (error.Index >= 0 && error.Index < toUpload.Count)
                    {
                        failedIndexes.Add(error.Index);
                    }
                }

                // Разделяем замеры на успешные и неуспешные
                var successfulMeasurements = new List<HumidityMeasurement>();
                var measurementsToDelete = new List<int>();

                for (int i = 0; i < toUpload.Count; i++)
                {
                    if (!failedIndexes.Contains(i))
                    {
                        measurementsToDelete.Add(toUpload[i].LocalId);
                        successfulMeasurements.Add(toUpload[i]);
                    }
                }

                // Удаляем из локальной БД только успешные
                if (measurementsToDelete.Any())
                {
                    await _localStorage.DeleteMeasurementsAsync(measurementsToDelete);
                }

                // Удаляем из UI-коллекции соответствующие элементы
                foreach (var m in successfulMeasurements)
                {
                    var itemToRemove = Measurements.FirstOrDefault(item => item.Measurement.LocalId == m.LocalId);
                    if (itemToRemove != null)
                    {
                        Measurements.Remove(itemToRemove);
                    }
                }

                // Формируем сообщение об ошибках, если они есть
                if (result.SkippedCount > 0 && result.Errors.Any())
                {
                    var errorDetails = string.Join("\n", result.Errors.Take(5).Select(e =>
                        $"• Запись #{e.Index + 1}: {e.Message}"));
                    if (result.Errors.Count() > 5)
                        errorDetails += "\n• ...и другие";

                    message.AppendLine("\n📝 Детали ошибок:");
                    message.AppendLine(errorDetails);
                }

                await Shell.Current.DisplayAlert("Результат выгрузки", message.ToString(), "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка сети",
                    "Не удалось связаться с сервером или получить корректный ответ. Замеры сохранены локально.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Критическая ошибка", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}