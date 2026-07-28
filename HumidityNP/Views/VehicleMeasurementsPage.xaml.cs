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
    /// Вызывается при появлении страницы (в том числе при возврате на неё).
    /// Подписываем ViewModel на события BLE-сервиса, чтобы получать актуальные данные с прибора,
    /// и перезагружаем список замеров, чтобы отобразить изменения, внесённые на других страницах
    /// (например, удаление или выгрузка замеров на AllMeasurementsPage).
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Подписываемся на события BLE при каждом появлении страницы.
        // Внутри метода есть защита от повторной подписки.
        _viewModel.SubscribeToBleEvents();

        // Перезагружаем список замеров из локального хранилища,
        // чтобы синхронизировать с изменениями, выполненными на других страницах.
        _viewModel.LoadDataCommand.Execute(null);
    }

    /// <summary>
    /// Вызывается при уходе со страницы (например, при переключении на другую вкладку).
    /// Отписываемся от событий BLE, чтобы не получать обновления, пока страница не видна.
    /// ВАЖНО: НЕ вызываем Dispose(), чтобы при возврате на страницу можно было снова подписаться.
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Только отписываемся от событий, но не уничтожаем ViewModel.
        // Это позволяет при возврате на страницу заново подписаться через OnAppearing().
        _viewModel.UnsubscribeFromBleEvents();
    }
}