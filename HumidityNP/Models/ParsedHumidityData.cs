using System;

namespace HumidityNP.Models
{
    public class ParsedHumidityData
    {
        public ReadingType Type { get; set; }
        public ushort MaterialCode { get; set; }
        public ScaleId Material => GetMaterialType(MaterialCode);
        public byte TemperatureF { get; set; }
        public double TemperatureC => (TemperatureF - 32) * 5.0 / 9.0;
        public double Value { get; set; }
        public SignType Sign { get; set; }
        public double Multiplier { get; set; }

        private static ScaleId GetMaterialType(ushort code)
        {
            if (Enum.IsDefined(typeof(ScaleId), code))
                return (ScaleId)code;
            return ScaleId.Unknown;
        }

        public override string ToString()
        {
            string signStr = Sign == SignType.Less ? "<" : (Sign == SignType.Greater ? ">" : "");
            string materialName = Material == ScaleId.Unknown ? $"0x{MaterialCode:X4}" : Material.ToString();
            return $"Влажность - {signStr}{Value:F1}%; Температура - {TemperatureC:F1}°C; Тип = {materialName};";
        }
    }

    public static class HumidityParser
    {
        public static ParsedHumidityData Parse(byte[] data)
        {
            if (data == null || data.Length < 6)
                throw new ArgumentException("Требуется минимум 6 байт", nameof(data));

            ReadingType type = (ReadingType)data[0];

            double multiplier = type switch
            {
                ReadingType.NonInsulatedWood or ReadingType.InsulatedWood or
                ReadingType.NonInsulatedDrywall or ReadingType.InsulatedDrywall => 0.1,
                ReadingType.NonInsulatedRef or ReadingType.InsulatedRef => 1.0,
                _ => throw new NotSupportedException($"Тип {type} не поддерживается")
            };

            ushort materialCode = (ushort)((data[2] << 8) | data[1]);
            byte temperatureF = data[3];

            byte lowByte = data[4];
            byte highByte = data[5];

            bool isGreater = (highByte & 0x80) != 0;
            bool isLess = (highByte & 0x40) != 0;

            int rawValue;
            SignType sign;

            if (isGreater)
            {
                sign = SignType.Greater;
                rawValue = ((highByte & 0x7F) << 8) | lowByte;
            }
            else if (isLess)
            {
                sign = SignType.Less;
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