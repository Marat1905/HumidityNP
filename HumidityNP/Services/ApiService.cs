using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HumidityNP.Models;
using Microsoft.Extensions.Logging;

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
            var url = "api/v1/vehicles/active/all";

            // Сервер вернет массив JSON, который десериализуется напрямую в List<Vehicle>
            var response = await _httpClient.GetFromJsonAsync<List<Vehicle>>(url);

            return response ?? new List<Vehicle>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка активных машин с API");

            // Возвращаем фейковые данные при ошибке сети, чтобы не ломать UI во время отладки
            return GetFakeVehicles();
        }
    }

    public async Task<bool> UploadMeasurementsAsync(List<HumidityMeasurement> measurements)
    {
        if (measurements == null || measurements.Count == 0)
            return true;

        try
        {
            var requests = new List<CreateMeasurementRequest>();
            foreach (var m in measurements)
            {
                // Безопасно преобразуем строковый ID из SQLite в Guid для API
                if (Guid.TryParse(m.VehicleId, out Guid vehicleGuid))
                {
                    requests.Add(new CreateMeasurementRequest
                    {
                        VehicleId = vehicleGuid,
                        HumidityValue = m.HumidityValue,
                        TemperatureC = m.TemperatureC,
                        MeasurementType = m.MeasurementType,
                        Material = m.Material,
                        Source = m.Source.ToString(), // Преобразуем enum в строку
                        Timestamp = new DateTimeOffset(m.Timestamp), // Преобразуем DateTime в DateTimeOffset
                        Sign = m.Sign
                    });
                }
                else
                {
                    _logger.LogWarning($"Неверный VehicleId: {m.VehicleId}. Пропуск замера.");
                }
            }

            if (requests.Count == 0)
                return false;

            var url = "api/v1/measurements/bulk";
            var json = JsonSerializer.Serialize(requests);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Успешно выгружено {requests.Count} замеров.");
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Ошибка выгрузки замеров. Статус: {response.StatusCode}. Ответ: {errorContent}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Исключение при выгрузке замеров на API");
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
                Date = new DateTime(2026, 5, 21, 15, 17, 40),
                ArrivalDate = new DateTime(2026, 5, 21, 10, 47, 38),
                EntryDate = new DateTime(2026, 5, 21, 11, 41, 19),
                ExitDate = new DateTime(2026, 5, 21, 15, 31, 55),
                Counterparty = "Тандер(Сургут)",
                WorkType = "Разгрузка",
                VehicleBrand = "FAW",
                VehiclePlate = "Н601УХ790",
                Trailer = "ТВ156477",
                Driver = "Иванов Иван Иванович",
                Loader = "",
                Expeditor = "ЭКОС ООО ТЛК",
                Department = ""
            },
            new Vehicle
            {
                Id = "v2",
                Number = "Я-9310099849",
                Date = new DateTime(2026, 5, 21, 16, 5, 10),
                ArrivalDate = new DateTime(2026, 5, 21, 12, 30, 0),
                EntryDate = new DateTime(2026, 5, 21, 13, 15, 22),
                ExitDate = null,
                Counterparty = "Магнит(Тюмень)",
                WorkType = "Разгрузка",
                VehicleBrand = "KAMAZ",
                VehiclePlate = "А777МР72",
                Trailer = "КТ88234",
                Driver = "Петров Пётр Петрович",
                Loader = "Сидоров С.С.",
                Expeditor = "Деловые Линии",
                Department = "Склад №3"
            },
            new Vehicle
            {
                Id = "v3",
                Number = "Я-9310099850",
                Date = new DateTime(2026, 5, 21, 14, 22, 5),
                ArrivalDate = new DateTime(2026, 5, 21, 9, 0, 0),
                EntryDate = new DateTime(2026, 5, 21, 9, 45, 10),
                ExitDate = new DateTime(2026, 5, 21, 14, 50, 0),
                Counterparty = "X5 Retail Group",
                WorkType = "Разгрузка",
                VehicleBrand = "MAN",
                VehiclePlate = "В123ОЕ174",
                Trailer = "РТ445566",
                Driver = "Кузнецов Алексей Владимирович",
                Loader = "",
                Expeditor = "ЖелДорЭкспедиция",
                Department = ""
            }
        };
    }
}