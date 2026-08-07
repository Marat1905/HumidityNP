using HumidityNP.Services;
using HumidityNP.ViewModels;
using HumidityNP.Views;
using MauiIcons.Material;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace HumidityNP;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMaterialMauiIcons()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // === РЕГИСТРАЦИЯ HTTP-КЛИЕНТА И API СЕРВИСА ===
        builder.Services.AddHttpClient<IApiService, ApiService>(client =>
        {
            // ВАЖНО: Укажите правильный базовый URL вашего API!
            // Для Android эмулятора: http://10.0.2.2:<порт_вашего_api> (например, 5000 или 7000)
            // Для iOS эмулятора: http://localhost:<порт_вашего_api>
            // Для реального физического устройства: http://<IP-адрес_компьютера_в_локальной_сети>:<порт>
            client.BaseAddress = new Uri("http://192.168.88.35:5000");
        });

        // Сервисы
        builder.Services.AddSingleton<IBleService, BleService>();
        builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();

        // ViewModels
        builder.Services.AddTransient<VehiclesViewModel>();
        builder.Services.AddTransient<VehicleMeasurementsViewModel>();
        builder.Services.AddTransient<AllMeasurementsViewModel>();
        builder.Services.AddTransient<UnloadViewModel>();
        builder.Services.AddTransient<AllUnloadsViewModel>();

        // Страницы
        builder.Services.AddTransient<AppShell>();
        builder.Services.AddTransient<SplashScreen>();
        builder.Services.AddTransient<VehiclesPage>();
        builder.Services.AddTransient<VehicleMeasurementsPage>();
        builder.Services.AddTransient<AllMeasurementsPage>();
        builder.Services.AddTransient<UnloadPage>();
        builder.Services.AddTransient<AllUnloadsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}