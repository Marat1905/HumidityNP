using HumidityNP.ViewModels;

namespace HumidityNP.Views;

/// <summary>
/// Главная страница приложения.
/// Отображает статус подключения к BLE-датчику, последние измерения
/// и лог событий. Является стартовой страницей при запуске приложения.
/// </summary>
public partial class MainPage : ContentPage
{
    /// <summary>
    /// Конструктор главной страницы.
    /// Инициализирует компоненты и устанавливает ViewModel в качестве контекста данных.
    /// </summary>
    /// <param name="viewModel">ViewModel для управления данными и командами страницы</param>
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    /// <summary>
    /// Вызывается при уходе со страницы.
    /// Освобождает ресурсы ViewModel (отписывается от событий BLE).
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Проверяем, реализует ли ViewModel интерфейс IDisposable
        if (BindingContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}