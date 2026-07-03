using WeatherWatch.Core;
using WeatherWatch.Core.Storage;
using Xunit;

namespace WeatherWatch.Core.Tests;

public sealed class JsonFileReadingStoreTests : IDisposable
{
    private readonly string _tempDirectory =
        Path.Combine(Path.GetTempPath(), "WeatherWatchTests", Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    private static WeatherReading CreateReading(int offsetMinutes = 0) => new(
        new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.FromHours(2)).AddMinutes(offsetMinutes),
        "central-station",
        21.5 + offsetMinutes,
        48.25);

    [Fact]
    public async Task GetAllAsync_MissingFile_ReturnsEmptyList()
    {
        var store = new JsonFileReadingStore(Path.Combine(_tempDirectory, "missing.json"));

        var readings = await store.GetAllAsync();

        Assert.Empty(readings);
    }

    [Fact]
    public async Task AppendAsync_ThenGetAllAsync_RoundTripsReading()
    {
        var filePath = Path.Combine(_tempDirectory, "readings.json");
        var store = new JsonFileReadingStore(filePath);
        var reading = CreateReading();

        await store.AppendAsync(reading);
        var readings = await store.GetAllAsync();

        var stored = Assert.Single(readings);
        Assert.Equal(reading, stored);
    }

    [Fact]
    public async Task AppendAsync_MissingDirectory_CreatesDirectory()
    {
        var filePath = Path.Combine(_tempDirectory, "nested", "deeper", "readings.json");
        var store = new JsonFileReadingStore(filePath);

        await store.AppendAsync(CreateReading());

        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public async Task AppendAsync_MultipleAppends_PreservesEarlierReadings()
    {
        var filePath = Path.Combine(_tempDirectory, "readings.json");
        var store = new JsonFileReadingStore(filePath);
        var first = CreateReading(0);
        var second = CreateReading(10);
        var third = CreateReading(20);

        await store.AppendAsync(first);
        await store.AppendAsync(second);
        await store.AppendAsync(third);
        var readings = await store.GetAllAsync();

        Assert.Equal([first, second, third], readings);
    }
}
