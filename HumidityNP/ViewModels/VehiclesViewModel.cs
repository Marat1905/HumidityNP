using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumidityNP.Models;
using HumidityNP.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HumidityNP.ViewModels;

/// <summary>
/// ViewModel для страницы списка машин.
/// Управляет загрузкой, отображением и поиском машин.
/// При выборе машины выполняет навигацию на страницу замеров.
/// </summary>
public partial class VehiclesViewModel : ObservableObject
{
    /// <summary>
    /// Сервис для работы с API (получение списка машин с сервера).
    /// </summary>
    private readonly IApiService _apiService;

    /// <summary>
    /// Сервис для работы с локальным хранилищем (кеш машин).
    /// </summary>
    private readonly ILocalStorageService _localStorage;

    /// <summary>
    /// Полный список машин, загруженный с сервера/из кеша.
    /// Используется как источник для фильтрации.
    /// </summary>
    private List<Vehicle> _allVehicles = new();

    /// <summary>
    /// Коллекция машин для отображения в UI.
    /// Может фильтроваться по поисковому запросу.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Vehicle> _vehicles = new();

    /// <summary>
    /// Выбранная машина в списке.
    /// При изменении выполняется навигация на страницу замеров.
    /// </summary>
    [ObservableProperty]
    private Vehicle _selectedVehicle;

    /// <summary>
    /// Флаг обновления (pull-to-refresh).
    /// </summary>
    [ObservableProperty]
    private bool _isRefreshing;

    /// <summary>
    /// Поисковый запрос для фильтрации списка машин.
    /// При изменении вызывает перерисовку списка.
    /// </summary>
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    /// <summary>
    /// Команда обновления списка машин (pull-to-refresh).
    /// </summary>
    public ICommand RefreshCommand { get; }

    /// <summary>
    /// Конструктор ViewModel.
    /// Инициализирует сервисы и запускает начальную загрузку данных.
    /// </summary>
    /// <param name="apiService">Сервис API для получения данных с сервера</param>
    /// <param name="localStorage">Сервис локального хранилища для кеширования</param>
    public VehiclesViewModel(IApiService apiService, ILocalStorageService localStorage)
    {
        _apiService = apiService;
        _localStorage = localStorage;

        RefreshCommand = new AsyncRelayCommand(LoadVehiclesAsync);

        // Загружаем данные при создании ViewModel
        LoadVehiclesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Реакция на изменение поискового запроса.
    /// Фильтрует список машин по номеру, марке или контрагенту.
    /// </summary>
    /// <param name="value">Новое значение поискового запроса</param>
    partial void OnSearchQueryChanged(string value)
    {
        FilterVehicles(value);
    }

    /// <summary>
    /// Реакция на выбор элемента в списке.
    /// Выполняет навигацию на страницу замеров для выбранной машины.
    /// </summary>
    /// <param name="value">Выбранная машина</param>
    partial void OnSelectedVehicleChanged(Vehicle value)
    {
        if (value != null)
        {
            // Переходим на страницу замеров, передавая ID машины
            Shell.Current.GoToAsync($"vehiclemeasurements?vehicleId={value.Id}");

            // Сбрасываем выделение, чтобы можно было выбрать ту же машину снова
            SelectedVehicle = null;
        }
    }

    /// <summary>
    /// Загружает список машин сначала из локального кеша,
    /// затем обновляет свежими данными с API.
    /// </summary>
    private async Task LoadVehiclesAsync()
    {
        IsRefreshing = true;
        try
        {
            // Сначала показываем данные из кеша (быстрый отклик)
            var cached = await _localStorage.GetCachedVehiclesAsync();
            if (cached.Any())
            {
                _allVehicles = cached.ToList();
                ApplyFilter();
            }

            // Затем загружаем свежие данные с API
            var fresh = await _apiService.GetVehiclesAsync();
            if (fresh.Any())
            {
                // Сохраняем в кеш
                await _localStorage.SaveVehiclesAsync(fresh);
                _allVehicles = fresh.ToList();
                ApplyFilter();
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

    /// <summary>
    /// Фильтрует список машин по поисковому запросу.
    /// Поиск выполняется по номеру, марке, гос. номеру и контрагенту.
    /// </summary>
    /// <param name="query">Поисковый запрос</param>
    private void FilterVehicles(string query)
    {
        ApplyFilter();
    }

    /// <summary>
    /// Применяет текущий поисковый запрос к полному списку машин
    /// и обновляет коллекцию Vehicles для отображения в UI.
    /// </summary>
    private void ApplyFilter()
    {
        IEnumerable<Vehicle> filtered = _allVehicles;

        // Если есть поисковый запрос — фильтруем
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            string query = SearchQuery.Trim().ToLowerInvariant();
            filtered = _allVehicles.Where(v =>
                v.Number.ToLowerInvariant().Contains(query) ||
                v.VehicleBrand.ToLowerInvariant().Contains(query) ||
                v.VehiclePlate.ToLowerInvariant().Contains(query) ||
                v.Counterparty.ToLowerInvariant().Contains(query) ||
                v.Driver.ToLowerInvariant().Contains(query));
        }

        // Обновляем ObservableCollection
        Vehicles.Clear();
        foreach (var v in filtered)
        {
            Vehicles.Add(v);
        }
    }
}