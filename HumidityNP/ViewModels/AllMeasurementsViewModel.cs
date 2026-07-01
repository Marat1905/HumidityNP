using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumidityNP.Models;
using HumidityNP.Services;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
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
            await Shell.Current.DisplayAlert("Выгрузка", "Нет замеров", "OK");
            return;
        }

        bool confirm = await Shell.Current.DisplayAlert("Выгрузка", $"Выгрузить все {toUpload.Count} замеров?", "Да", "Нет");
        if (!confirm) return;

        try
        {
            if (await _apiService.UploadMeasurementsAsync(toUpload))
            {
                var ids = toUpload.Select(m => m.LocalId).ToList();
                await _localStorage.DeleteMeasurementsAsync(ids);
                foreach (var m in toUpload)
                    Measurements.Remove(m);
                await Shell.Current.DisplayAlert("Успех", "Все замеры выгружены", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Ошибка", "Не удалось выгрузить", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
        }
    }
}