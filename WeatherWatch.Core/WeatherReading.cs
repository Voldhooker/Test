namespace WeatherWatch.Core;

/// <summary>
/// A validated weather reading. Instances can only be produced via <see cref="Create"/>,
/// so every reading is guaranteed to satisfy the domain invariants.
/// </summary>
public sealed record WeatherReading
{
    // Private constructor: direct instantiation would bypass validation in Create.
    private WeatherReading()
    {
    }

    public required string Location { get; init; }

    public required DateTimeOffset TimestampUtc { get; init; }

    public required double TemperatureCelsius { get; init; }

    public required double WindSpeedKmh { get; init; }

    public required int HumidityPercent { get; init; }

    public static WeatherReading Create(
        string location,
        DateTimeOffset timestampUtc,
        double temperatureCelsius,
        double windSpeedKmh,
        int humidityPercent)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            throw new ArgumentException("location cannot be empty or whitespace.", nameof(location));
        }

        if (temperatureCelsius is < -90 or > 60)
        {
            throw new ArgumentException(
                "temperatureCelsius must be between -90 and 60 degrees Celsius.",
                nameof(temperatureCelsius));
        }

        if (windSpeedKmh < 0)
        {
            throw new ArgumentException("windSpeedKmh cannot be negative.", nameof(windSpeedKmh));
        }

        if (humidityPercent is < 0 or > 100)
        {
            throw new ArgumentException(
                "humidityPercent must be between 0 and 100.",
                nameof(humidityPercent));
        }

        return new WeatherReading
        {
            Location = location,
            TimestampUtc = timestampUtc,
            TemperatureCelsius = temperatureCelsius,
            WindSpeedKmh = windSpeedKmh,
            HumidityPercent = humidityPercent,
        };
    }
}
