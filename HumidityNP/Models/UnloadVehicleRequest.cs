namespace HumidityNP.Models
{
    /// <summary>
    /// Запрос на фиксацию разгрузки машины для отправки на сервер.
    /// </summary>
    public class UnloadVehicleRequest
    {
        public int BaleCount { get; set; }
        public int DamagedBaleCount { get; set; }
        public double WeightKg { get; set; }
        public string StackNumber { get; set; } = string.Empty;
    }
}