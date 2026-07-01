using HumidityNP.ViewModels;

namespace HumidityNP.Views;

[QueryProperty(nameof(VehicleId), "vehicleId")]
public partial class VehicleMeasurementsPage : ContentPage
{
    private readonly VehicleMeasurementsViewModel _viewModel;

    public VehicleMeasurementsPage(VehicleMeasurementsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public string VehicleId
    {
        set => _viewModel.VehicleId = value;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Dispose();
    }
}