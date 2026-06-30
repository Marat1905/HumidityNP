namespace HumidityNP.Enums
{
    /// <summary>
    /// Источник замера влажности.
    /// </summary>
    public enum MeasurementSource
    {
        /// <summary>
        /// Автоматически от BLE-датчика.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Вручную пользователем.
        /// </summary>
        Manual = 1
    }
}
