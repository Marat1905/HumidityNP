using HumidityNP.Models;

namespace HumidityNP.Extensions
{
    /// <summary>
    /// Статический парсер для преобразования сырых байтов BLE-пакета
    /// в объект <see cref="ParsedHumidityData"/>.
    /// </summary>
    public static class HumidityParser
    {
        /// <summary>
        /// Разбирает массив байтов (минимум 6) в структурированные данные влажности.
        /// </summary>
        /// <param name="data">Массив байтов, полученный от характеристики New Reading.</param>
        /// <returns>Объект <see cref="ParsedHumidityData"/> с заполненными полями.</returns>
        /// <exception cref="ArgumentException">Если массив null или содержит менее 6 байт.</exception>
        /// <exception cref="NotSupportedException">Если тип измерения (<see cref="ReadingType"/>) не поддерживается.</exception>
        public static ParsedHumidityData Parse(byte[] data)
        {
            if (data == null || data.Length < 6)
                throw new ArgumentException("Требуется минимум 6 байт", nameof(data));

            ReadingType type = (ReadingType)data[0];

            // Определение множителя в зависимости от типа
            double multiplier = type switch
            {
                ReadingType.NonInsulatedWood or ReadingType.InsulatedWood or
                ReadingType.NonInsulatedDrywall or ReadingType.InsulatedDrywall => 0.1,
                ReadingType.NonInsulatedRef or ReadingType.InsulatedRef => 1.0,
                _ => throw new NotSupportedException($"Тип {type} не поддерживается")
            };

            // Код материала (little-endian: байт 2 — старший, байт 1 — младший)
            ushort materialCode = (ushort)((data[2] << 8) | data[1]);
            byte temperatureF = data[3];

            byte lowByte = data[4];
            byte highByte = data[5];

            // Флаги выхода за диапазон
            bool isGreater = (highByte & 0x80) != 0;
            bool isLess = (highByte & 0x40) != 0;

            int rawValue;
            SignType sign;

            if (isGreater)
            {
                sign = SignType.Greater;
                // Старший байт без флага Greater
                rawValue = ((highByte & 0x7F) << 8) | lowByte;
            }
            else if (isLess)
            {
                sign = SignType.Less;
                // При Less используется только младший байт
                rawValue = lowByte;
            }
            else
            {
                sign = SignType.None;
                rawValue = (highByte << 8) | lowByte;
            }

            double value = rawValue * multiplier;

            return new ParsedHumidityData
            {
                Type = type,
                MaterialCode = materialCode,
                TemperatureF = temperatureF,
                Value = value,
                Sign = sign,
                Multiplier = multiplier
            };
        }
    }
}