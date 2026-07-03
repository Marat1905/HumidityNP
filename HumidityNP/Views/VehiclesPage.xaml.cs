using HumidityNP.ViewModels;

namespace HumidityNP.Views;

/// <summary>
/// Страница списка машин.
/// Отображает все машины, доступные на площадке, с возможностью поиска.
/// При нажатии на машину переходит на страницу замеров для этой машины.
/// </summary>
public partial class VehiclesPage : ContentPage
{
    /// <summary>
    /// Конструктор страницы.
    /// Инициализирует компоненты и устанавливает ViewModel в качестве контекста данных.
    /// </summary>
    /// <param name="viewModel">ViewModel для управления данными и командами страницы</param>
    public VehiclesPage(VehiclesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}