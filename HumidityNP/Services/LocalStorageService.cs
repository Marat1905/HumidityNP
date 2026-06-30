using HumidityNP.Models;
using SQLite;
using System.Text.Json;

namespace HumidityNP.Services;

/// <summary>
/// Локальное хранилище на базе SQLite.
/// Данные сохраняются на устройстве и доступны без сети.
/// </summary>
public class LocalStorageService : ILocalStorageService
{
    private SQLiteAsyncConnection? _db;
    private readonly string _dbPath;

    private const string VehiclesCacheKey = "vehicles_cache";

    public LocalStorageService()
    {
        _dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "humiditynp.db3");
    }

    public async Task InitializeAsync()
    {
        if (_db != null) return;

        _db = new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        await _db.CreateTableAsync<HumidityMeasurement>();
    }

    public async Task SaveMeasurementAsync(HumidityMeasurement measurement)
    {
        await EnsureDbAsync();
        measurement.LocalId = 0; // сброс для AutoIncrement
        await _db!.InsertAsync(measurement);
    }

    public async Task<List<HumidityMeasurement>> GetAllMeasurementsAsync()
    {
        await EnsureDbAsync();
        return await _db!.Table<HumidityMeasurement>()
            .OrderByDescending(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task<List<HumidityMeasurement>> GetMeasurementsByVehicleAsync(string vehicleId)
    {
        await EnsureDbAsync();
        return await _db!.Table<HumidityMeasurement>()
            .Where(m => m.VehicleId == vehicleId)
            .OrderByDescending(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task<List<HumidityMeasurement>> GetPendingUploadsAsync()
    {
        await EnsureDbAsync();
        return await _db!.Table<HumidityMeasurement>()
            .ToListAsync();
    }

    public async Task DeleteMeasurementAsync(int localId)
    {
        await EnsureDbAsync();
        await _db!.DeleteAsync<HumidityMeasurement>(localId);
    }

    public async Task DeleteMeasurementsAsync(List<int> localIds)
    {
        await EnsureDbAsync();
        foreach (var id in localIds)
        {
            await _db!.DeleteAsync<HumidityMeasurement>(id);
        }
    }

    public async Task SaveVehiclesAsync(List<Vehicle> vehicles)
    {
        var json = JsonSerializer.Serialize(vehicles);
        await SecureStorage.SetAsync(VehiclesCacheKey, json);
    }

    public async Task<List<Vehicle>> GetCachedVehiclesAsync()
    {
        var json = await SecureStorage.GetAsync(VehiclesCacheKey);
        if (string.IsNullOrEmpty(json))
            return new List<Vehicle>();

        try
        {
            return JsonSerializer.Deserialize<List<Vehicle>>(json) ?? new List<Vehicle>();
        }
        catch
        {
            return new List<Vehicle>();
        }
    }

    private async Task EnsureDbAsync()
    {
        if (_db == null)
            await InitializeAsync();
    }
}