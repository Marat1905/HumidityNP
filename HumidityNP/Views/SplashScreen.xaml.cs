namespace HumidityNP.Views;

/// <summary>
/// Анимированная заставка приложения с Lottie-анимацией.
/// Показывает анимацию из файла NP.json, затем автоматически
/// переходит к основному приложению (AppShell).
/// 
/// Вместо фиксированной задержки (Task.Delay) используется ожидание
/// реального завершения анимации, что обеспечивает одинаковое поведение
/// на всех устройствах независимо от их производительности.
/// </summary>
public partial class SplashScreen : ContentPage
{
    /// <summary>
    /// Максимальное время показа заставки в миллисекундах.
    /// Защита от зависания в случае, если анимация не загрузилась.
    /// </summary>
    private const int MaxSplashDurationMs = 6000;

    /// <summary>
    /// Флаг, предотвращающий повторный переход к основному приложению.
    /// </summary>
    private bool _isNavigating;

    public SplashScreen()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Вызывается при появлении страницы.
    /// Запускает Lottie-анимацию и ждёт её завершения перед переходом к AppShell.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Защита от повторного вызова OnAppearing
        // (может происходить при возврате из фона или других lifecycle-событиях)
        if (_isNavigating) return;
        _isNavigating = true;

        try
        {
            // Настраиваем анимацию: проиграть ровно один раз (без повторов)
            LottieAnimation.RepeatCount = 0;

            // Сбрасываем прогресс в начало
            LottieAnimation.Progress = TimeSpan.Zero;

            var startTime = DateTime.UtcNow;
            var maxTime = TimeSpan.FromMilliseconds(MaxSplashDurationMs);

            // Цикл ожидания: выходим либо по завершении анимации, либо по таймауту
            while (true)
            {
                var elapsed = DateTime.UtcNow - startTime;

                // Защита от бесконечного ожидания
                if (elapsed > maxTime)
                    break;

                // Проверяем, загрузилась ли анимация и завершилась ли она
                var duration = LottieAnimation.Duration;
                if (duration > TimeSpan.Zero && LottieAnimation.Progress >= duration)
                    break;

                // Ждём немного перед следующей проверкой
                await Task.Delay(30);
            }

            // Переходим к основному приложению
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage = new AppShell();
            });
        }
        catch (Exception)
        {
            // В случае любой ошибки всё равно переходим к основному приложению
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage = new AppShell();
            });
        }
    }
}