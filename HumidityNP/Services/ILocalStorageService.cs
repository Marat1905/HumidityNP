using System.Collections.Generic;
using System.Threading.Tasks;
using HumidityNP.Models;

namespace HumidityNP.Services;

/// <summary>
/// Сервис локального хранения данных (SQLite).
/// </summary>
public interface ILocalStorageService
{
    /// <summary>Инициализация базы данных</summary>
    Task InitializeAsync();

    /// <summary>Сохранить замер влажности</summary>
    Task SaveMeasurementAsync(HumidityMeasurement measurement);

    /// <summary>Получить все замеры</summary>
    Task<List<HumidityMeasurement>> GetAllMeasurementsAsync();

    /// <summary>Получить замеры для конкретной машины</summary>
    Task<List<HumidityMeasurement>> GetMeasurementsByVehicleAsync(string vehicleId);

    /// <summary>Получить невыгруженные замеры</summary>
    Task<List<HumidityMeasurement>> GetPendingUploadsAsync();

    /// <summary>Удалить замер по локальному ID</summary>
    Task DeleteMeasurementAsync(int localId);

    /// <summary>Удалить список замеров (после успешной выгрузки)</summary>
    Task DeleteMeasurementsAsync(List<int> localIds);

    /// <summary>Сохранить список машин (кеш)</summary>
    Task SaveVehiclesAsync(List<Vehicle> vehicles);

    /// <summary>Получить кешированные машины</summary>
    Task<List<Vehicle>> GetCachedVehiclesAsync();

    // ---- МЕТОДЫ ДЛЯ РАЗГРУЗКИ ----

    /// <summary>Сохранить информацию о разгрузке</summary>
    Task SaveUnloadInfoAsync(UnloadInfo unload);

    /// <summary>Получить информацию о разгрузке по ID машины</summary>
    Task<UnloadInfo?> GetUnloadInfoByVehicleAsync(string vehicleId);

    /// <summary>Получить информацию о разгрузке по локальному ID</summary>
    Task<UnloadInfo?> GetUnloadInfoByLocalIdAsync(int localId);

    /// <summary>Удалить разгрузку по локальному ID</summary>
    Task DeleteUnloadInfoAsync(int localId);

    /// <summary>Удалить разгрузку для конкретной машины</summary>
    Task DeleteUnloadInfoForVehicleAsync(string vehicleId);

    /// <summary>Получить все сохранённые разгрузки</summary>
    Task<List<UnloadInfo>> GetAllUnloadInfosAsync();
}