using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumidityNP.Models;
using HumidityNP.Services;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Text;
using System.Windows.Input;

namespace HumidityNP.ViewModels;

public partial class AllMeasurementsViewModel : ObservableObject
{
    private readonly ILocalStorageService _localStorage;
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<HumidityMeasurement> _measurements = new();

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
        DeleteMeasurementCommand = new AsyncRelayCommand<HumidityMeasurement>(DeleteMeasurementAsync);
        UploadAllCommand = new AsyncRelayCommand(UploadAllAsync);

        LoadMeasurementsAsync().ConfigureAwait(false);
    }

    private async Task LoadMeasurementsAsync()
    {
        IsRefreshing = true;
        try
        {
            var all = await _localStorage.GetAllMeasurementsAsync();
            Measurements.Clear();
            foreach (var m in all.OrderByDescending(m => m.Timestamp))
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

    private async Task UploadAllAsync()
    {
        var toUpload = Measurements.ToList();
        if (!toUpload.Any())
        {
            await Shell.Current.DisplayAlert("Выгрузка", "Нет замеров для выгрузки", "OK");
            return;
        }

        bool confirm = await Shell.Current.DisplayAlert("Выгрузка", $"Выгрузить {toUpload.Count} замеров на сервер?", "Да", "Нет");
        if (!confirm) return;

        IsBusy = true; // Если у вас есть такое свойство для индикатора загрузки
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

                // 1. Определяем индексы замеров, которые завершились ошибкой
                var failedIndexes = new HashSet<int>();
                foreach (var error in result.Errors)
                {
                    if (error.Index >= 0 && error.Index < toUpload.Count)
                    {
                        failedIndexes.Add(error.Index);
                    }
                }

                // 2. Разделяем замеры на успешные и неуспешные
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

                // 3. Удаляем из локальной БД только успешные
                if (measurementsToDelete.Any())
                {
                    await _localStorage.DeleteMeasurementsAsync(measurementsToDelete);
                }

                // 4. Удаляем из UI-коллекции (ObservableCollection)
                foreach (var m in successfulMeasurements)
                {
                    Measurements.Remove(m);
                }

                // 5. Формируем сообщение об ошибках, если они есть
                if (result.SkippedCount > 0 && result.Errors.Any())
                {
                    var errorDetails = string.Join("\n", result.Errors.Take(5).Select(e => $"• Запись #{e.Index + 1}: {e.Message}"));
                    if (result.Errors.Count() > 5)
                        errorDetails += "\n• ...и другие";

                    message.AppendLine("\n📝 Детали ошибок:");
                    message.AppendLine(errorDetails);
                }

                await Shell.Current.DisplayAlert("Результат выгрузки", message.ToString(), "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Ошибка сети", "Не удалось связаться с сервером или получить корректный ответ. Замеры сохранены локально.", "OK");
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