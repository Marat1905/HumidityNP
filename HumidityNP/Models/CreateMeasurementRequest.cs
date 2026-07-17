using System;
using System.Text.Json.Serialization;

namespace HumidityNP.Models;

public class CreateMeasurementRequest
{
    [JsonPropertyName("vehicleId")]
    public Guid VehicleId { get; set; }

    [JsonPropertyName("humidityValue")]
    public double HumidityValue { get; set; }

    [JsonPropertyName("temperatureC")]
    public double TemperatureC { get; set; }

    [JsonPropertyName("measurementType")]
    public string MeasurementType { get; set; } = string.Empty;

    [JsonPropertyName("material")]
    public string Material { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("sign")]
    public string Sign { get; set; } = string.Empty;
}