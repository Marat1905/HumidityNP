using HumidityNP.ViewModels;

namespace HumidityNP.Views;

public partial class AllMeasurementsPage : ContentPage
{
    private readonly AllMeasurementsViewModel _viewModel;

    public AllMeasurementsPage(AllMeasurementsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Автоматически обновляем список при переходе на вкладку
        _viewModel.RefreshCommand.Execute(null);
    }
}