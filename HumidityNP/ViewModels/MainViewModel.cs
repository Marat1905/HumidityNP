using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumidityNP.Enums;
using HumidityNP.Models;
using HumidityNP.Services;

namespace HumidityNP.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        private readonly IBleService _bleService;

        [ObservableProperty]
        private string _connectionStatus = "Отключено";

        [ObservableProperty]
        private bool _isConnected;

        [ObservableProperty]
        private string _measurementType = "-";

        [ObservableProperty]
        private string _material = "-";

        [ObservableProperty]
        private string _temperature = "-";

        [ObservableProperty]
        private string _humidity = "-";

        public ObservableCollection<string> LogMessages { get; } = new();

        // Внедрение через конструктор
        public MainViewModel(IBleService bleService)
        {
            _bleService = bleService;
            _bleService.OnStatusChanged += OnStatusChanged;
            _bleService.OnDataReceived += OnDataReceived;
        }

        private void OnStatusChanged(string status)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ConnectionStatus = status;
                IsConnected = _bleService.IsConnected;
                AddLog(status);
            });
        }

        private void OnDataReceived(ParsedHumidityData data)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                MeasurementType = GetMeasurementTypeName(data.Type);
                Material = data.Material.ToString();
                Temperature = $"{data.TemperatureC:F1}°C ({data.TemperatureF}°F)";

                string sign = data.Sign == SignType.Less ? "<" :
                             data.Sign == SignType.Greater ? ">" : "";
                Humidity = $"{sign}{data.Value:F1}%";

                AddLog($"Данные: {data}");
            });
        }

        private string GetMeasurementTypeName(ReadingType type)
        {
            return type switch
            {
                ReadingType.NonInsulatedWood => "Дерево (неизолир.)",
                ReadingType.InsulatedWood => "Дерево (изолир.)",
                ReadingType.NonInsulatedDrywall => "Гипсокартон (неизолир.)",
                ReadingType.InsulatedDrywall => "Гипсокартон (изолир.)",
                ReadingType.NonInsulatedRef => "Относительное (неизолир.)",
                ReadingType.InsulatedRef => "Относительное (изолир.)",
                ReadingType.RHT => "Температура/Влажность",
                _ => "Неизвестно"
            };
        }

        private void AddLog(string message)
        {
            LogMessages.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");
            if (LogMessages.Count > 50)
                LogMessages.RemoveAt(LogMessages.Count - 1);
        }

        [RelayCommand]
        private async Task ConnectAsync()
        {
            if (IsConnected)
            {
                await _bleService.DisconnectAsync();
            }
            else
            {
                _ = _bleService.StartAutoConnectAsync();
            }
        }

        public void Dispose()
        {
            _bleService.OnStatusChanged -= OnStatusChanged;
            _bleService.OnDataReceived -= OnDataReceived;
            _bleService.Dispose();
        }
    }
}