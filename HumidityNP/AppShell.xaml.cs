using HumidityNP.Views;

namespace HumidityNP;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Регистрируем маршрут для страницы замеров конкретной машины
        Routing.RegisterRoute("vehiclemeasurements", typeof(VehicleMeasurementsPage));
    }
}