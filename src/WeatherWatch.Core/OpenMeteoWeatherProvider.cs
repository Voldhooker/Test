using System.Text.Json;
using System.Text.Json.Serialization;

namespace WeatherWatch.Core;

/// <summary>
/// <see cref="IWeatherProvider"/> backed by the public Open-Meteo geocoding and forecast APIs.
/// HTTP and deserialization failures are infrastructure faults and propagate as exceptions.
/// </summary>
public sealed class OpenMeteoWeatherProvider : IWeatherProvider
{
    private const string GeocodingUrlFormat = "https://geocoding-api.open-meteo.com/v1/search?name={0}&count=1";
    private const string ForecastUrlFormat = "https://api.open-meteo.com/v1/forecast?latitude={0}&longitude={1}&current=temperature_2m,relative_humidity_2m,wind_speed_10m";

    private readonly HttpClient _httpClient;

    public OpenMeteoWeatherProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherReading> GetCurrentAsync(string city, CancellationToken ct = default)
    {
        var (latitude, longitude) = await ResolveCoordinatesAsync(city, ct);
        var current = await FetchCurrentWeatherAsync(latitude, longitude, ct);

        return WeatherReading.Create(city, current.Temperature, current.RelativeHumidity, current.WindSpeed);
    }

    private async Task<(double Latitude, double Longitude)> ResolveCoordinatesAsync(string city, CancellationToken ct)
    {
        var url = string.Format(GeocodingUrlFormat, Uri.EscapeDataString(city));
        using var response = await _httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        var geocoding = await JsonSerializer.DeserializeAsync<GeocodingResponse>(stream, cancellationToken: ct);

        var match = geocoding?.Results?.FirstOrDefault()
            ?? throw new InvalidOperationException($"Unknown city: '{city}'.");

        return (match.Latitude, match.Longitude);
    }

    private async Task<CurrentWeather> FetchCurrentWeatherAsync(double latitude, double longitude, CancellationToken ct)
    {
        var url = string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            ForecastUrlFormat,
            latitude,
            longitude);

        using var response = await _httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        var forecast = await JsonSerializer.DeserializeAsync<ForecastResponse>(stream, cancellationToken: ct);

        return forecast?.Current
            ?? throw new InvalidOperationException("Open-Meteo forecast response did not contain current weather data.");
    }

    private sealed record GeocodingResponse(
        [property: JsonPropertyName("results")] IReadOnlyList<GeocodingResult>? Results);

    private sealed record GeocodingResult(
        [property: JsonPropertyName("latitude")] double Latitude,
        [property: JsonPropertyName("longitude")] double Longitude);

    private sealed record ForecastResponse(
        [property: JsonPropertyName("current")] CurrentWeather? Current);

    private sealed record CurrentWeather(
        [property: JsonPropertyName("temperature_2m")] double Temperature,
        [property: JsonPropertyName("relative_humidity_2m")] double RelativeHumidity,
        [property: JsonPropertyName("wind_speed_10m")] double WindSpeed);
}
