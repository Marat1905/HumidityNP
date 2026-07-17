using System;
using System.Collections.Generic;

namespace HumidityNP.Models;

/// <summary>
/// Результат массовой загрузки замеров.
/// </summary>
public class BulkMeasurementResult
{
    /// <summary>
    /// Количество успешно созданных замеров.
    /// </summary>
    public int CreatedCount { get; set; }

    /// <summary>
    /// Количество пропущенных замеров (из-за ошибок валидации или отсутствия машины).
    /// </summary>
    public int SkippedCount { get; set; }

    /// <summary>
    /// Список ошибок для каждого пропущенного замера.
    /// </summary>
    public IEnumerable<MeasurementBulkError> Errors { get; set; } = new List<MeasurementBulkError>();
}

/// <summary>
/// Детали ошибки для одного замера при массовой загрузке.
/// </summary>
public class MeasurementBulkError
{
    /// <summary>
    /// Порядковый номер записи во входном списке (начиная с 0).
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Идентификатор машины, указанный в запросе.
    /// </summary>
    public Guid VehicleId { get; set; }

    /// <summary>
    /// Текст ошибки.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}