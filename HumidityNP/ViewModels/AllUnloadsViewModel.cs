using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumidityNP.Models;
using HumidityNP.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HumidityNP.ViewModels;

/// <summary>
/// ViewModel для страницы всех локальных разгрузок.
/// Загружает список UnloadInfo из локального хранилища, дополняя их информацией о машинах.
/// Позволяет удалять записи и переходить к редактированию.
/// </summary>
public partial class AllUnloadsViewModel : ObservableObject
{
    private readonly ILocalStorageService _localStorage;

    [ObservableProperty]
    private ObservableCollection<UnloadDisplayItem> _unloads = new();

    [ObservableProperty]
    private bool _isRefreshing;

    public ICommand RefreshCommand { get; }
    public ICommand DeleteUnloadCommand { get; }
    public ICommand EditUnloadCommand { get; }

    public AllUnloadsViewModel(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;

        RefreshCommand = new AsyncRelayCommand(LoadUnloadsAsync);
        DeleteUnloadCommand = new AsyncRelayCommand<UnloadDisplayItem>(DeleteUnloadAsync);
        EditUnloadCommand = new AsyncRelayCommand<UnloadDisplayItem>(EditUnloadAsync);

        _ = LoadUnloadsAsync();
    }

    /// <summary>
    /// Загружает все разгрузки, добавляет информацию о машинах из кеша.
    /// </summary>
    private async Task LoadUnloadsAsync()
    {
        IsRefreshing = true;
        try
        {
            var unloads = await _localStorage.GetAllUnloadInfosAsync();
            var vehicles = await _localStorage.GetCachedVehiclesAsync();
            var vehicleDict = vehicles.ToDictionary(v => v.Id, v => v);

            var items = new List<UnloadDisplayItem>();

            foreach (var u in unloads.OrderByDescending(u => u.Timestamp))
            {
                Vehicle vehicle = null;
                if (!string.IsNullOrEmpty(u.VehicleId) && vehicleDict.TryGetValue(u.VehicleId, out var v))
                {
                    vehicle = v;
                }

                string displayInfo = vehicle != null ? $"{vehicle.Number} ({vehicle.VehiclePlate})" : u.VehicleId;

                items.Add(new UnloadDisplayItem
                {
                    UnloadInfo = u,
                    VehicleDisplayInfo = displayInfo
                });
            }

            Unloads.Clear();
            foreach (var item in items)
            {
                Unloads.Add(item);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки разгрузок: {ex.Message}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    /// <summary>
    /// Удаление разгрузки с подтверждением.
    /// </summary>
    private async Task DeleteUnloadAsync(UnloadDisplayItem item)
    {
        if (item == null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Удаление",
            $"Удалить разгрузку для {item.VehicleDisplayInfo} от {item.UnloadInfo.Timestamp.ToLocalTime():dd.MM.yyyy HH:mm}?",
            "Да", "Нет");

        if (confirm)
        {
            await _localStorage.DeleteUnloadInfoAsync(item.UnloadInfo.LocalId);
            Unloads.Remove(item);
        }
    }

    /// <summary>
    /// Переход к редактированию разгрузки.
    /// Передаём на страницу редактирования (UnloadPage) параметр localId.
    /// </summary>
    private async Task EditUnloadAsync(UnloadDisplayItem item)
    {
        if (item == null) return;

        // Передаём localId разгрузки на страницу редактирования
        await Shell.Current.GoToAsync($"unload?localId={item.UnloadInfo.LocalId}");
    }
}

/// <summary>
/// Элемент для отображения разгрузки в списке.
/// </summary>
public class UnloadDisplayItem
{
    public UnloadInfo UnloadInfo { get; set; }
    public string VehicleDisplayInfo { get; set; } = string.Empty;

    public int BaleCount => UnloadInfo.BaleCount;
    public int DamagedBaleCount => UnloadInfo.DamagedBaleCount;
    public double WeightKg => UnloadInfo.WeightKg;
    public string StackNumber => UnloadInfo.StackNumber;
    public bool IsUploaded => UnloadInfo.IsUploaded;
    public DateTimeOffset LocalTimestamp => UnloadInfo.Timestamp.ToLocalTime();
}