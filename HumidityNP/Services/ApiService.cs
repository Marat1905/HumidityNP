using HumidityNP.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HumidityNP.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Vehicle>> GetActiveVehiclesAsync()
    {
        try
        {
            // Обращаемся к новому эндпоинту без пагинации
            var url = "humidity/api/v1/vehicles/active/all";

            // Сервер вернет массив JSON, который десериализуется напрямую в List<Vehicle>
            var response = await _httpClient.GetFromJsonAsync<List<Vehicle>>(url);

            return response ?? new List<Vehicle>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка активных машин с API");
            return default(List<Vehicle>);
            // Возвращаем фейковые данные при ошибке сети, чтобы не ломать UI во время отладки
            //return GetFakeVehicles();
        }
    }

    public async Task<BulkMeasurementResult?> UploadMeasurementsAsync(List<HumidityMeasurement> measurements)
    {
        try
        {
            var requests = new List<CreateMeasurementRequest>();

            foreach (var m in measurements)
            {
                // Преобразуем строковый VehicleId в Guid, как ожидает сервер
                if (Guid.TryParse(m.VehicleId, out var vehicleGuid))
                {
                    requests.Add(new CreateMeasurementRequest
                    {
                        VehicleId = vehicleGuid,
                        HumidityValue = m.HumidityValue,
                        TemperatureC = m.TemperatureC,
                        MeasurementType = m.MeasurementType,
                        Material = m.Material,
                        Sign = m.Sign,
                        Source = m.Source,
                        Timestamp = m.Timestamp
                    });
                }
            }

            if (requests.Count == 0)
            {
                return new BulkMeasurementResult
                {
                    CreatedCount = 0,
                    SkippedCount = measurements.Count,
                    Errors = new List<MeasurementBulkError>
                    {
                        new MeasurementBulkError { Message = "Некорректный VehicleId во всех записях" }
                    }
                };
            }

            var url = "humidity/api/v1/measurements/bulk";
            var json = JsonSerializer.Serialize(requests);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<BulkMeasurementResult>(resultJson, options);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[API] Ошибка выгрузки: {response.StatusCode}, {errorContent}");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] Исключение при выгрузке: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UnloadVehicleAsync(Guid vehicleId, UnloadVehicleRequest request)
    {
        try
        {
            var url = $"humidity/api/v1/vehicles/{vehicleId}/unload";
            var response = await _httpClient.PostAsJsonAsync(url, request);
            if (response.IsSuccessStatusCode)
                return true;

            // Читаем тело ошибки для диагностики
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Ошибка при вызове UnloadVehicle для {VehicleId}: {StatusCode}, {Error}",
                vehicleId, response.StatusCode, errorContent);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при вызове UnloadVehicle для {VehicleId}", vehicleId);
            return false;
        }
    }

    /// <summary>
    /// Фейковые данные для тестирования.
    /// </summary>
    private static List<Vehicle> GetFakeVehicles()
    {
        return new List<Vehicle>
        {
            new Vehicle
            {
                Id = "v1",
                Number = "Я-9310099848",
                Date = new DateTimeOffset(2026, 5, 21, 15, 17, 40, TimeSpan.Zero),
                EntryDate = new DateTimeOffset(2026, 5, 21, 10, 47, 38, TimeSpan.Zero),
                ExitDate = new DateTimeOffset(2026, 5, 21, 15, 31, 55, TimeSpan.Zero),
                Counterparty = "Тандер(Сургут)",
                Inn = "1234567890",
                VehicleBrand = "FAW",
                VehiclePlate = "Н601УХ790",
                Trailer = "ТВ156477",
                Driver = "Иванов Иван Иванович",
                MeasurementsCount = 5
            },
            new Vehicle
            {
                Id = "v2",
                Number = "Я-9310099849",
                Date = new DateTimeOffset(2026, 5, 21, 16, 5, 10, TimeSpan.Zero),
                EntryDate = new DateTimeOffset(2026, 5, 21, 12, 30, 0, TimeSpan.Zero),
                ExitDate = null,
                Counterparty = "Магнит(Тюмень)",
                Inn = "0987654321",
                VehicleBrand = "KAMAZ",
                VehiclePlate = "А777МР72",
                Trailer = "КТ88234",
                Driver = "Петров Пётр Петрович",
                MeasurementsCount = 3
            },
            new Vehicle
            {
                Id = "v3",
                Number = "Я-9310099850",
                Date = new DateTimeOffset(2026, 5, 21, 14, 22, 5, TimeSpan.Zero),
                EntryDate = new DateTimeOffset(2026, 5, 21, 9, 0, 0, TimeSpan.Zero),
                ExitDate = new DateTimeOffset(2026, 5, 21, 14, 50, 0, TimeSpan.Zero),
                Counterparty = "X5 Retail Group",
                Inn = "1122334455",
                VehicleBrand = "MAN",
                VehiclePlate = "В123ОЕ174",
                Trailer = "РТ445566",
                Driver = "Кузнецов Алексей Владимирович",
                MeasurementsCount = 8
            }
        };
    }
}