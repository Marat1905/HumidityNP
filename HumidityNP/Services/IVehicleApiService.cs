using System.Collections.Generic;
using System.Threading.Tasks;
using HumidityNP.Models;

namespace HumidityNP.Services;

/// <summary>
/// Сервис для получения списка машин с сервера (API).
/// </summary>
public interface IVehicleApiService
{
    /// <summary>
    /// Получить список машин, въезжающих на площадку.
    /// </summary>
    Task<List<Vehicle>> GetVehiclesAsync();
}