namespace WeatherWatch.Core;

/// <summary>
/// Represents an immutable weather reading with validated properties.
/// </summary>
public record WeatherReading
{
    public required string Location { get; init; }
    public required DateTime TimestampUtc { get; init; }
    public required decimal TemperatureCelsius { get; init; }
    public required decimal WindSpeedKmh { get; init; }
    public required int HumidityPercent { get; init; }

    /// <summary>
    /// Creates a new weather reading with validated input ranges.
    /// </summary>
    /// <param name="location">The location name. Cannot be empty or whitespace.</param>
    /// <param name="timestampUtc">The UTC timestamp of the reading.</param>
    /// <param name="temperatureCelsius">Temperature in Celsius. Must be between -90 and 60.</param>
    /// <param name="windSpeedKmh">Wind speed in km/h. Must be >= 0.</param>
    /// <param name="humidityPercent">Humidity percentage. Must be between 0 and 100.</param>
    /// <returns>A validated WeatherReading instance.</returns>
    /// <exception cref="ArgumentException">Thrown when any parameter fails validation.</exception>
    public static WeatherReading Create(
        string location,
        DateTime timestampUtc,
        decimal temperatureCelsius,
        decimal windSpeedKmh,
        int humidityPercent)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            throw new ArgumentException("Location cannot be empty or whitespace.", nameof(location));
        }

        if (temperatureCelsius < -90 || temperatureCelsius > 60)
        {
            throw new ArgumentException(
                "Temperature must be between -90 and 60 degrees Celsius.",
                nameof(temperatureCelsius));
        }

        if (windSpeedKmh < 0)
        {
            throw new ArgumentException(
                "Wind speed must be greater than or equal to 0 km/h.",
                nameof(windSpeedKmh));
        }

        if (humidityPercent < 0 || humidityPercent > 100)
        {
            throw new ArgumentException(
                "Humidity must be between 0 and 100 percent.",
                nameof(humidityPercent));
        }

        return new WeatherReading
        {
            Location = location,
            TimestampUtc = timestampUtc,
            TemperatureCelsius = temperatureCelsius,
            WindSpeedKmh = windSpeedKmh,
            HumidityPercent = humidityPercent
        };
    }
}
