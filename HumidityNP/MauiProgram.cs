using Microsoft.Extensions.Logging;
using HumidityNP.Views;
using HumidityNP.ViewModels;
using HumidityNP.Services;

namespace HumidityNP;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Сервисы
        builder.Services.AddSingleton<IBleService, BleService>();
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();

        // ViewModels
        builder.Services.AddTransient<VehiclesViewModel>();
        builder.Services.AddTransient<VehicleMeasurementsViewModel>();
        builder.Services.AddTransient<AllMeasurementsViewModel>();

        // Страницы
        builder.Services.AddTransient<AppShell>();
        builder.Services.AddTransient<VehiclesPage>();
        builder.Services.AddTransient<VehicleMeasurementsPage>();
        builder.Services.AddTransient<AllMeasurementsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}