namespace HumidityNP;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // 1. Применяем сохраненную тему при старте (если она была выбрана ранее)
        if (Preferences.ContainsKey("AppTheme"))
        {
            var savedTheme = (AppTheme)Preferences.Get("AppTheme", (int)AppTheme.Unspecified);
            this.UserAppTheme = savedTheme;
        }

        // 2. Показываем анимированную заставку, затем переходим к основному приложению
        MainPage = new Views.SplashScreen();
    }
}