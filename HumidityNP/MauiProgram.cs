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

        // === Регистрация сервисов ===
        builder.Services.AddSingleton<IBleService, BleService>();
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();

        // === Регистрация ViewModels ===
        builder.Services.AddSingleton<MainViewModel>();          // для MainPage


        // === Регистрация страниц ===
        builder.Services.AddTransient<AppShell>();                  // корневой Shell
        builder.Services.AddTransient<MainPage>();                  // страница списка машин
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}