using System.Text.Json;

namespace WeatherWatch.Core.Storage;

/// <summary>JSON-file-backed reading store using read-modify-write to preserve history.</summary>
public sealed class JsonFileReadingStore : IReadingStore
{
    // WriteIndented keeps the file human-readable for debugging; case-insensitive
    // property matching makes deserialization tolerant of hand-edited files.
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    private readonly string _filePath;

    public JsonFileReadingStore(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
    }

    public async Task AppendAsync(WeatherReading reading, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reading);

        var readings = new List<WeatherReading>(await GetAllAsync(cancellationToken)) { reading };

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, readings, SerializerOptions, cancellationToken);
    }

    public async Task<IReadOnlyList<WeatherReading>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_filePath);
        var readings = await JsonSerializer.DeserializeAsync<List<WeatherReading>>(
            stream, SerializerOptions, cancellationToken);

        return readings ?? [];
    }
}
