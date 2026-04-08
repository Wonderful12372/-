namespace WeatherApp.Services;

public interface IDataStorageService
{
    Task SaveLastCityAsync(string city);
    Task<string> LoadLastCityAsync();
    Task SaveThemeAsync(string theme);
    Task<string> LoadThemeAsync();
    Task SaveHistoryAsync(List<string> history);
    Task<List<string>> LoadHistoryAsync();
}