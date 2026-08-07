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

        // Маршрут для страницы разгрузки
        Routing.RegisterRoute("unload", typeof(UnloadPage));

        // Устанавливаем правильный текст кнопки при старте
        UpdateThemeText();
    }

    /// <summary>
    /// Обработчик нажатия на кнопку переключения темы
    /// </summary>
    private void OnThemeToggleClicked(object sender, EventArgs e)
    {
        // Получаем текущую тему
        var currentTheme = Application.Current.UserAppTheme;

        // Циклически переключаем: Светлая -> Тёмная -> Авто (как в системе) -> Светлая
        if (currentTheme == AppTheme.Light)
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
        }
        else if (currentTheme == AppTheme.Dark)
        {
            Application.Current.UserAppTheme = AppTheme.Unspecified; // Следовать системным настройкам
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Light;
        }

        // Сохраняем выбор пользователя, чтобы он не сбросился после перезапуска
        Preferences.Set("AppTheme", (int)Application.Current.UserAppTheme);

        // Обновляем текст иконки на кнопке
        UpdateThemeText();
    }

    /// <summary>
    /// Обновляет текст кнопки в зависимости от выбранной темы
    /// </summary>
    private void UpdateThemeText()
    {
        var currentTheme = Application.Current.UserAppTheme;

        switch (currentTheme)
        {
            case AppTheme.Light:
                ThemeToggleItem.Text = "☀️";
                break;
            case AppTheme.Dark:
                ThemeToggleItem.Text = "🌙";
                break;
            default:
                ThemeToggleItem.Text = "🌓";
                break;
        }
    }
}