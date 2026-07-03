namespace WeatherWatch.Core;

/// <summary>
/// A single point-in-time weather observation for a city.
/// </summary>
public sealed record WeatherReading
{
    private WeatherReading(string city, double temperatureCelsius, double humidityPercent, double windSpeedKmh)
    {
        City = city;
        TemperatureCelsius = temperatureCelsius;
        HumidityPercent = humidityPercent;
        WindSpeedKmh = windSpeedKmh;
    }

    public string City { get; }

    public double TemperatureCelsius { get; }

    public double HumidityPercent { get; }

    public double WindSpeedKmh { get; }

    /// <summary>
    /// Creates a reading, enforcing the domain invariants in one place so callers
    /// (e.g. weather providers) do not have to duplicate validation.
    /// </summary>
    public static WeatherReading Create(string city, double temperatureCelsius, double humidityPercent, double windSpeedKmh)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City must not be empty.", nameof(city));
        }

        if (humidityPercent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(humidityPercent), humidityPercent, "Humidity must be between 0 and 100 percent.");
        }

        if (windSpeedKmh < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(windSpeedKmh), windSpeedKmh, "Wind speed must not be negative.");
        }

        return new WeatherReading(city.Trim(), temperatureCelsius, humidityPercent, windSpeedKmh);
    }
}
