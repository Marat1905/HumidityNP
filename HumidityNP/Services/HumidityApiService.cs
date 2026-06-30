using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HumidityNP.Models;

namespace HumidityNP.Services;

/// <summary>
/// Сервис выгрузки замеров на сервер.
/// </summary>
public class HumidityApiService : IHumidityApiService
{
    private readonly HttpClient _httpClient;

    // TODO: Заменить на реальный URL сервера
    private const string ApiBaseUrl = "https://api.example.com";

    public HumidityApiService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public async Task<bool> UploadMeasurementsAsync(List<HumidityMeasurement> measurements)
    {
        try
        {
            // TODO: Заменить на реальный API-запрос
            // var json = JsonSerializer.Serialize(measurements);
            // var content = new StringContent(json, Encoding.UTF8, "application/json");
            // var response = await _httpClient.PostAsync($"{ApiBaseUrl}/humidity/upload", content);
            // return response.IsSuccessStatusCode;

            // Фейковая задержка для имитации сети
            await Task.Delay(1000);

            System.Diagnostics.Debug.WriteLine(
                $"[API] Выгружено {measurements.Count} замеров (фейк)");

            // Имитация успеха
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] Ошибка выгрузки: {ex.Message}");
            return false;
        }
    }
}