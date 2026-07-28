using HumidityNP.Models;

namespace HumidityNP.Models
{
    /// <summary>
    /// Обёртка для отображения замера на странице всех замеров.
    /// Содержит сам замер и вычисляемые поля на основе данных о машине.
    /// </summary>
    public class MeasurementDisplayItem
    {
        /// <summary>Исходный замер влажности.</summary>
        public HumidityMeasurement Measurement { get; set; }

        /// <summary>Отображаемая информация о машине: номер пропуска и госномер в скобках.</summary>
        public string DisplayVehicleInfo { get; set; }

        /// <summary>Номер пропуска машины.</summary>
        public string VehicleNumber { get; set; }

        /// <summary>Государственный номер без пробелов, в верхнем регистре.</summary>
        public string VehiclePlate { get; set; }
    }
}