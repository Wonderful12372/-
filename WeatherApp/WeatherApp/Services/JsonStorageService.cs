using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherApp.Services;

public class JsonStorageService : IDataStorageService
{
    private record AppSettings(string LastCity, string Theme, List<string> History);
    private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

    private async Task<AppSettings> LoadAsync()
    {
        if (!File.Exists(_path)) return new AppSettings("", "Light", new());
        var json = await File.ReadAllTextAsync(_path);
        return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings("", "Light", new());
    }

    private async Task SaveAsync(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_path, json);
    }

    public async Task<string> LoadLastCityAsync() => (await LoadAsync()).LastCity;
    public async Task SaveLastCityAsync(string city) { var s = await LoadAsync(); await SaveAsync(s with { LastCity = city }); }

    public async Task<string> LoadThemeAsync() => (await LoadAsync()).Theme;
    public async Task SaveThemeAsync(string theme) { var s = await LoadAsync(); await SaveAsync(s with { Theme = theme }); }

    public async Task<List<string>> LoadHistoryAsync() => (await LoadAsync()).History;
    public async Task SaveHistoryAsync(List<string> history) { var s = await LoadAsync(); await SaveAsync(s with { History = history }); }
}