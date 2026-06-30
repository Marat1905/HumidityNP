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
    /// Получить список машин, въезжающих на площадку.
    /// </summary>
    Task<List<Vehicle>> GetVehiclesAsync();

    /// <summary>
    /// Выгрузить список замеров на сервер.
    /// Возвращает true при успехе.
    /// </summary>
    Task<bool> UploadMeasurementsAsync(List<HumidityMeasurement> measurements);
}