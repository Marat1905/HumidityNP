using HumidityNP.ViewModels;

namespace HumidityNP.Views;

/// <summary>
/// Страница замеров влажности для конкретной машины.
/// Отображает информацию о машине, текущие данные с BLE-датчика,
/// позволяет зафиксировать замер или ввести его вручную.
/// Также показывает историю всех замеров для данной машины.
/// </summary>
[QueryProperty(nameof(VehicleId), "vehicleId")]
public partial class VehicleMeasurementsPage : ContentPage
{
    /// <summary>
    /// ViewModel для управления данными и командами страницы.
    /// </summary>
    private readonly VehicleMeasurementsViewModel _viewModel;

    /// <summary>
    /// Конструктор страницы.
    /// Инициализирует компоненты и устанавливает ViewModel в качестве контекста данных.
    /// </summary>
    /// <param name="viewModel">ViewModel для управления данными и командами страницы</param>
    public VehicleMeasurementsPage(VehicleMeasurementsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    /// <summary>
    /// ID машины, передаётся через навигационный параметр.
    /// При установке значения запускается загрузка данных для этой машины.
    /// </summary>
    public string VehicleId
    {
        set => _viewModel.VehicleId = value;
    }

    /// <summary>
    /// Вызывается при уходе со страницы.
    /// Освобождает ресурсы ViewModel (отписывается от событий BLE).
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Dispose();
    }
}