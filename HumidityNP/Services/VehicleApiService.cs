using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HumidityNP.Models;

namespace HumidityNP.Services;

/// <summary>
/// Сервис получения машин. Сейчас возвращает фейковые данные.
/// В будущем здесь будет HTTP-запрос к API.
/// </summary>
public class VehicleApiService : IVehicleApiService
{
    public async Task<List<Vehicle>> GetVehiclesAsync()
    {
        // Имитация задержки сети
        await Task.Delay(500);

        // TODO: Заменить на реальный HTTP-запрос к API
        // var client = new HttpClient();
        // var response = await client.GetStringAsync("https://api.example.com/vehicles");
        // return JsonSerializer.Deserialize<List<Vehicle>>(response);

        return GetFakeVehicles();
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
                WorkType = "Погрузка",
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