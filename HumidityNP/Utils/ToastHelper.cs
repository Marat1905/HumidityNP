using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace HumidityNP.Utils;

public static class ToastHelper
{
    public static async Task ShowErrorAsync(string message)
    {
        var options = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#D32F2F"),
            TextColor = Colors.White, // В новых версиях TextColor заменен на MessageTextColor
            CornerRadius = 8,
            ActionButtonTextColor = Colors.White
        };

        // Передаем options и duration как именованные аргументы
        var snackbar = Snackbar.Make(message, visualOptions: options, duration: TimeSpan.FromSeconds(2));
        await snackbar.Show();
    }

    public static async Task ShowSuccessAsync(string message)
    {
        var options = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#388E3C"),
            TextColor = Colors.White,
            CornerRadius = 8,
            ActionButtonTextColor = Colors.White
        };

        var snackbar = Snackbar.Make(message, visualOptions: options, duration: TimeSpan.FromSeconds(2));
        await snackbar.Show();
    }

    public static async Task ShowInfoAsync(string message)
    {
        var options = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#1976D2"),
            TextColor = Colors.White,
            CornerRadius = 8,
            ActionButtonTextColor = Colors.White

        };

        var snackbar = Snackbar.Make(message, visualOptions: options, duration: TimeSpan.FromSeconds(2));
        await snackbar.Show();
    }

    public static async Task ShowWarningAsync(string message)
    {
        var options = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#FFA000"),
            TextColor = Colors.White,
            CornerRadius = 8,
            ActionButtonTextColor = Colors.White
        };

        var snackbar = Snackbar.Make(message, visualOptions: options, duration: TimeSpan.FromSeconds(2));
        await snackbar.Show();
    }
}