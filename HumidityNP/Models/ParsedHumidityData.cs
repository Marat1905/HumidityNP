using System;

namespace HumidityNP.Models
{
    
    /// <summary>
    /// Представляет результат разбора 6-байтного пакета с измерением влажности
    /// для типов 0x02–0x07 (штыревые измерения).
    /// </summary>
    /// <remarks>
    /// Формат данных (6 байт):
    /// - Byte 0: ReadingType
    /// - Bytes 1-2: MaterialCode (ushort, little-endian)
    /// - Byte 3: TemperatureF (в °F)
    /// - Bytes 4-5: Value с флагами Less/Greater (см. <see cref="SignType"/>)
    /// </remarks>
    public class ParsedHumidityData
    {
        /// <summary>Тип измерения (определяет множитель и материал).</summary>
        public ReadingType Type { get; set; }

        /// <summary>Код материала (сырое значение из пакета).</summary>
        public ushort MaterialCode { get; set; }

        /// <summary>
        /// Материал (порода древесины или другой материал), определённый по коду.
        /// Если код не найден в <see cref="ScaleId"/>, возвращает <see cref="ScaleId.Unknown"/>.
        /// </summary>
        public ScaleId Material => GetMaterialType(MaterialCode);

        /// <summary>Температура в градусах Фаренгейта (целое значение из пакета).</summary>
        public byte TemperatureF { get; set; }

        /// <summary>Температура в градусах Цельсия (вычисляется из <see cref="TemperatureF"/>).</summary>
        public double TemperatureC => (TemperatureF - 32) * 5.0 / 9.0;

        /// <summary>Числовое значение влажности (с учётом множителя).</summary>
        public double Value { get; set; }

        /// <summary>Тип знака (Less/Greater/None), указывает на выход за диапазон.</summary>
        public SignType Sign { get; set; }

        /// <summary>Множитель, применённый к сырому значению (зависит от <see cref="Type"/>).</summary>
        public double Multiplier { get; set; }

        /// <summary>
        /// Возвращает идентификатор шкалы по числовому коду.
        /// </summary>
        /// <param name="code">Код материала (ushort).</param>
        /// <returns>Соответствующий <see cref="ScaleId"/>, или <see cref="ScaleId.Unknown"/>.</returns>
        private static ScaleId GetMaterialType(ushort code)
        {
            if (Enum.IsDefined(typeof(ScaleId), code))
                return (ScaleId)code;
            return ScaleId.Unknown;
        }

        /// <summary>
        /// Возвращает строковое представление данных для отображения в UI или логе.
        /// </summary>
        public override string ToString()
        {
            string signStr = Sign == SignType.Less ? "<" : (Sign == SignType.Greater ? ">" : "");
            string materialName = Material == ScaleId.Unknown ? $"0x{MaterialCode:X4}" : Material.ToString();
            return $"Влажность - {signStr}{Value:F1}%; Температура - {TemperatureC:F1}°C; Тип = {materialName};";
        }
    }
}