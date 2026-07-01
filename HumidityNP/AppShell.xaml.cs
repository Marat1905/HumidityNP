using HumidityNP.Views;
using MauiIcons.Core;

namespace HumidityNP;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        _ = new MauiIcon();
        // Регистрируем маршрут для страницы замеров конкретной машины
        Routing.RegisterRoute("vehiclemeasurements", typeof(VehicleMeasurementsPage));
    }
}