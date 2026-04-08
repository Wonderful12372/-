using WeatherApp.Models;

namespace WeatherApp.Services;

public interface IWeatherService
{
    Task<WeatherInfo> GetWeatherAsync(string city);
}