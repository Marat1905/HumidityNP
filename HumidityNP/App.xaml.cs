using HumidityNP.Views;
using Microsoft.Extensions.DependencyInjection;

namespace HumidityNP;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        MainPage = serviceProvider.GetRequiredService<MainPage>();
    }
}