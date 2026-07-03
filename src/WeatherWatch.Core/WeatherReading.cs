namespace WeatherWatch.Core;

/// <summary>A single weather observation captured at a station.</summary>
public sealed record WeatherReading(
    DateTimeOffset Timestamp,
    string Station,
    double TemperatureCelsius,
    double HumidityPercent);
