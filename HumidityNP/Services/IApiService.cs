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
    /// Выгрузить список замеров на сервер.
    /// Возвращает результат массовой операции или null при сетевой ошибке.
    /// </summary>
    Task<BulkMeasurementResult?> UploadMeasurementsAsync(List<HumidityMeasurement> measurements);
}