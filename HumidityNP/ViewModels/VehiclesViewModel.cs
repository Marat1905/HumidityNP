using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumidityNP.Models;
using HumidityNP.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HumidityNP.ViewModels;

public partial class VehiclesViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly ILocalStorageService _localStorage;

    [ObservableProperty]
    private ObservableCollection<Vehicle> _vehicles = new();

    [ObservableProperty]
    private Vehicle _selectedVehicle;

    [ObservableProperty]
    private bool _isRefreshing;

    public ICommand RefreshCommand { get; }

    public VehiclesViewModel(IApiService apiService, ILocalStorageService localStorage)
    {
        _apiService = apiService;
        _localStorage = localStorage;
        RefreshCommand = new AsyncRelayCommand(LoadVehiclesAsync);
        LoadVehiclesAsync().ConfigureAwait(false);
    }

    // Реакция на выбор элемента
    partial void OnSelectedVehicleChanged(Vehicle value)
    {
        if (value != null)
        {
            Shell.Current.GoToAsync($"vehiclemeasurements?vehicleId={value.Id}");
            // Сбрасываем выделение, чтобы можно было выбрать ту же машину снова
            SelectedVehicle = null;
        }
    }

    private async Task LoadVehiclesAsync()
    {
        IsRefreshing = true;
        try
        {
            // Сначала из кеша
            var cached = await _localStorage.GetCachedVehiclesAsync();
            if (cached.Any())
            {
                Vehicles.Clear();
                foreach (var v in cached)
                    Vehicles.Add(v);
            }

            // Затем свежие из API
            var fresh = await _apiService.GetVehiclesAsync();
            if (fresh.Any())
            {
                await _localStorage.SaveVehiclesAsync(fresh);
                Vehicles.Clear();
                foreach (var v in fresh)
                    Vehicles.Add(v);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки машин: {ex.Message}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }
}