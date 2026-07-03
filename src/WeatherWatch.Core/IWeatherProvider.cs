namespace WeatherWatch.Core;

/// <summary>
/// Source of current weather observations, keyed by city name.
/// </summary>
public interface IWeatherProvider
{
    /// <summary>
    /// Fetches the current weather for <paramref name="city"/>.
    /// Throws <see cref="InvalidOperationException"/> when the city cannot be resolved.
    /// </summary>
    Task<WeatherReading> GetCurrentAsync(string city, CancellationToken ct = default);
}
