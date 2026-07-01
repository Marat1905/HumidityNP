using HumidityNP.ViewModels;

namespace HumidityNP.Views;

public partial class VehiclesPage : ContentPage
{
    public VehiclesPage(VehiclesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}