using HumidityNP.ViewModels;

namespace HumidityNP.Views;

/// <summary>
/// Страница отображения всех локально сохранённых разгрузок.
/// Позволяет просматривать, редактировать и удалять записи.
/// </summary>
public partial class AllUnloadsPage : ContentPage
{
    private readonly AllUnloadsViewModel _viewModel;

    public AllUnloadsPage(AllUnloadsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    /// <summary>
    /// При появлении страницы загружаем список разгрузок.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshCommand.Execute(null);
    }
}