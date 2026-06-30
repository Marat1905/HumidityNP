using System.Collections.Generic;
using System.Threading.Tasks;
using HumidityNP.Models;

namespace HumidityNP.Services;

/// <summary>
/// Сервис для выгрузки замеров влажности на сервер.
/// </summary>
public interface IHumidityApiService
{
    /// <summary>
    /// Выгрузить список замеров на сервер.
    /// Возвращает true при успехе.
    /// </summary>
    Task<bool> UploadMeasurementsAsync(List<HumidityMeasurement> measurements);
}