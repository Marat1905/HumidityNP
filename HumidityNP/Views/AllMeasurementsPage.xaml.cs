using HumidityNP.ViewModels;

namespace HumidityNP.Views;

/// <summary>
/// Страница отображения всех локальных замеров влажности.
/// Показывает список всех замеров, сохранённых на устройстве,
/// которые ещё не были выгружены на сервер.
/// Позволяет выгрузить все замеры на сервер одним нажатием.
/// </summary>
public partial class AllMeasurementsPage : ContentPage
{
    /// <summary>
    /// ViewModel для управления данными и командами страницы.
    /// </summary>
    private readonly AllMeasurementsViewModel _viewModel;

    /// <summary>
    /// Конструктор страницы.
    /// Инициализирует компоненты и устанавливает ViewModel в качестве контекста данных.
    /// </summary>
    /// <param name="viewModel">ViewModel для управления данными и командами страницы</param>
    public AllMeasurementsPage(AllMeasurementsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    /// <summary>
    /// Вызывается при появлении страницы на экране.
    /// Автоматически обновляет список замеров при переходе на эту вкладку,
    /// чтобы показать самые актуальные данные.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Автоматически обновляем список при переходе на вкладку
        _viewModel.RefreshCommand.Execute(null);
    }
}