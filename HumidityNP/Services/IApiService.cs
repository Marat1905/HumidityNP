using System.Collections.Generic;
using System.Threading.Tasks;
using HumidityNP.Models;

namespace HumidityNP.Services;

/// <summary>
/// Сервис для получения списка машин с сервера (API).
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Получить полный список активных машин, въезжающих или находящихся на площадке.
    /// </summary>
    Task<List<Vehicle>> GetActiveVehiclesAsync();

    /// <summary>
    /// Массовая выгрузка списка замеров на сервер.
    /// </summary>
    Task<bool> UploadMeasurementsAsync(List<HumidityMeasurement> measurements);
}