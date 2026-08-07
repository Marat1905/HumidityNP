using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumidityNP.Models;
using HumidityNP.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HumidityNP.ViewModels;

/// <summary>
/// ViewModel для страницы ввода/редактирования разгрузки.
/// Загружает данные о машине и существующую разгрузку (если есть),
/// позволяет сохранить или обновить запись.
/// </summary>
public partial class UnloadViewModel : ObservableObject
{
    private readonly ILocalStorageService _localStorage;
    private readonly IApiService _apiService; // не используется напрямую, но для единообразия оставим

    private string _vehicleId;
    private int _editingLocalId; // 0 = новая запись, >0 = редактирование

    [ObservableProperty]
    private string _vehicleNumber = string.Empty;

    [ObservableProperty]
    private string _vehicleDisplayName = string.Empty;

    [ObservableProperty]
    private string _counterparty = string.Empty;

    [ObservableProperty]
    private string _baleCount = string.Empty;

    [ObservableProperty]
    private string _damagedBaleCount = string.Empty;

    [ObservableProperty]
    private string _weightKg = string.Empty;

    [ObservableProperty]
    private string _stackNumber = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public ICommand SaveUnloadCommand { get; }

    public UnloadViewModel(ILocalStorageService localStorage, IApiService apiService)
    {
        _localStorage = localStorage;
        _apiService = apiService;

        SaveUnloadCommand = new AsyncRelayCommand(SaveUnloadAsync);
    }

    public string VehicleId
    {
        get => _vehicleId;
        set
        {
            if (_vehicleId != value)
            {
                _vehicleId = value;
                _editingLocalId = 0; // новая запись
                _ = LoadDataAsync();
            }
        }
    }

    /// <summary>
    /// Загружает существующую разгрузку для редактирования по localId.
    /// </summary>
    public async Task LoadForEditAsync(int localId)
    {
        if (localId <= 0) return;

        _editingLocalId = localId;
        await LoadDataAsync();
    }

    /// <summary>
    /// Загружает информацию о машине и существующую разгрузку (если есть).
    /// Если _editingLocalId > 0, загружает именно эту запись.
    /// Иначе загружает по VehicleId (для новой записи).
    /// </summary>
    private async Task LoadDataAsync()
    {
        if (string.IsNullOrEmpty(VehicleId) && _editingLocalId == 0) return;

        IsBusy = true;
        try
        {
            // Определяем, какую машину показывать
            string targetVehicleId = VehicleId;

            // Если редактируем существующую запись, получаем VehicleId из неё
            UnloadInfo existingUnload = null;
            if (_editingLocalId > 0)
            {
                existingUnload = await _localStorage.GetUnloadInfoByLocalIdAsync(_editingLocalId);
                if (existingUnload != null)
                {
                    targetVehicleId = existingUnload.VehicleId;
                }
            }

            // Загружаем машину
            if (!string.IsNullOrEmpty(targetVehicleId))
            {
                var vehicles = await _localStorage.GetCachedVehiclesAsync();
                var vehicle = vehicles.FirstOrDefault(v => v.Id == targetVehicleId);
                if (vehicle != null)
                {
                    VehicleNumber = vehicle.Number;
                    VehicleDisplayName = vehicle.DisplayName;
                    Counterparty = vehicle.Counterparty;
                }
            }

            // Если редактируем, заполняем поля из существующей записи
            if (existingUnload != null)
            {
                BaleCount = existingUnload.BaleCount.ToString();
                DamagedBaleCount = existingUnload.DamagedBaleCount.ToString();
                WeightKg = existingUnload.WeightKg.ToString("0.##");
                StackNumber = existingUnload.StackNumber;
                // Сохраняем VehicleId для корректного обновления
                _vehicleId = existingUnload.VehicleId;
            }
            else
            {
                // Если новая запись, но уже есть разгрузка для этой машины, загружаем её
                if (!string.IsNullOrEmpty(VehicleId))
                {
                    var unload = await _localStorage.GetUnloadInfoByVehicleAsync(VehicleId);
                    if (unload != null)
                    {
                        BaleCount = unload.BaleCount.ToString();
                        DamagedBaleCount = unload.DamagedBaleCount.ToString();
                        WeightKg = unload.WeightKg.ToString("0.##");
                        StackNumber = unload.StackNumber;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Ошибка", $"Не удалось загрузить данные: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Сохраняет разгрузку локально.
    /// Если _editingLocalId > 0, обновляет существующую запись (удаляет старую, создаёт новую с тем же VehicleId).
    /// Иначе создаёт новую.
    /// </summary>
    private async Task SaveUnloadAsync()
    {
        if (string.IsNullOrEmpty(VehicleId))
        {
            await Shell.Current.DisplayAlert("Ошибка", "Не выбрана машина", "OK");
            return;
        }

        // Парсинг введённых значений
        if (!int.TryParse(BaleCount, out int baleCount) || baleCount < 0)
        {
            await Shell.Current.DisplayAlert("Ошибка", "Введите корректное количество тюков (целое неотрицательное число)", "OK");
            return;
        }

        if (!int.TryParse(DamagedBaleCount, out int damagedCount) || damagedCount < 0)
        {
            await Shell.Current.DisplayAlert("Ошибка", "Введите корректное количество порванных тюков (целое неотрицательное число)", "OK");
            return;
        }

        if (!double.TryParse(WeightKg, out double weight) || weight < 0)
        {
            await Shell.Current.DisplayAlert("Ошибка", "Введите корректный вес (неотрицательное число)", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(StackNumber))
        {
            await Shell.Current.DisplayAlert("Ошибка", "Введите номер штабеля", "OK");
            return;
        }

        // Если редактируем, сначала удаляем старую запись
        if (_editingLocalId > 0)
        {
            await _localStorage.DeleteUnloadInfoAsync(_editingLocalId);
        }
        else
        {
            // Если новая запись, но для этой машины уже есть, удаляем её (чтобы избежать дублей)
            var existing = await _localStorage.GetUnloadInfoByVehicleAsync(VehicleId);
            if (existing != null)
            {
                await _localStorage.DeleteUnloadInfoForVehicleAsync(VehicleId);
            }
        }

        // Создаём новую запись
        var newUnload = new UnloadInfo
        {
            VehicleId = VehicleId,
            BaleCount = baleCount,
            DamagedBaleCount = damagedCount,
            WeightKg = weight,
            StackNumber = StackNumber.Trim(),
            Timestamp = DateTimeOffset.UtcNow,
            IsUploaded = false
        };

        await _localStorage.SaveUnloadInfoAsync(newUnload);

        await Shell.Current.DisplayAlert("Успех", "Данные разгрузки сохранены локально", "OK");

        // Возвращаемся на предыдущую страницу
        await Shell.Current.GoToAsync("..");
    }
}