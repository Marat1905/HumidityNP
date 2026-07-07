namespace HumidityNP.Views;

/// <summary>
/// Анимированная заставка приложения с Lottie-анимацией.
/// Показывает анимацию из файла NP.json, затем автоматически
/// переходит к основному приложению (AppShell).
/// </summary>
public partial class SplashScreen : ContentPage
{
    /// <summary>
    /// Время показа заставки в миллисекундах (3 секунды).
    /// </summary>
    private const int SplashDurationMs = 3000;

    public SplashScreen()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Вызывается при появлении страницы.
    /// Запускает таймер, после которого происходит переход к AppShell.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Запускаем Lottie-анимацию с самого начала
        LottieAnimation.Progress = TimeSpan.Zero;

        // Ждём заданное время, пока пользователь смотрит на заставку
        await Task.Delay(SplashDurationMs);

        // Переходим к основному приложению
        Application.Current.MainPage = new AppShell();
    }
}