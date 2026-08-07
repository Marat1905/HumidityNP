using SQLite;
using System;

namespace HumidityNP.Models
{
    /// <summary>
    /// Информация о разгрузке машины: количество тюков, порванных, вес, штабель.
    /// Хранится локально до отправки на сервер.
    /// </summary>
    [Table("unload_info")]
    public class UnloadInfo
    {
        [PrimaryKey, AutoIncrement]
        public int LocalId { get; set; }

        [Indexed]
        public string VehicleId { get; set; } = string.Empty;

        public int BaleCount { get; set; }
        public int DamagedBaleCount { get; set; }
        public double WeightKg { get; set; }
        public string StackNumber { get; set; } = string.Empty;

        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Флаг, что разгрузка уже отправлена на сервер (используется для пометки).
        /// </summary>
        public bool IsUploaded { get; set; }
    }
}