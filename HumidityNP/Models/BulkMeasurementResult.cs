using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HumidityNP.Models;

/// <summary>
/// Результат массовой загрузки замеров.
/// </summary>
public class BulkMeasurementResult
{
    /// <summary>
    /// Количество успешно созданных замеров.
    /// </summary>
    [JsonPropertyName("createdCount")]
    public int CreatedCount { get; set; }

    /// <summary>
    /// Количество пропущенных замеров (из-за ошибок валидации или отсутствия машины).
    /// </summary>
    [JsonPropertyName("skippedCount")]
    public int SkippedCount { get; set; }

    /// <summary>
    /// Список ошибок для каждого пропущенного замера.
    /// </summary>
    [JsonPropertyName("errors")]
    public List<MeasurementBulkError> Errors { get; set; } = new();
}

/// <summary>
/// Детали ошибки для одного замера при массовой загрузке.
/// </summary>
public class MeasurementBulkError
{
    /// <summary>
    /// Порядковый номер записи во входном списке (начиная с 0).
    /// </summary>
   [JsonPropertyName("index")]
    public int Index { get; set; }

    /// <summary>
    /// Идентификатор машины, указанный в запросе.
    /// </summary>
    [JsonPropertyName("vehicleId")]
    public Guid VehicleId { get; set; }

    /// <summary>
    /// Текст ошибки.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}