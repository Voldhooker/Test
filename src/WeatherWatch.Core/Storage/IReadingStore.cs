namespace WeatherWatch.Core.Storage;

/// <summary>Persists weather readings and returns the accumulated history.</summary>
public interface IReadingStore
{
    Task AppendAsync(WeatherReading reading, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WeatherReading>> GetAllAsync(CancellationToken cancellationToken = default);
}
