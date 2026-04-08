using System.Net.Http.Json;
using WeatherApp.Models;
using System.Net.Http;

namespace WeatherApp.Services;

public class WeatherApiService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "e25d38069066de983ded0407fabd62ba";
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

    public WeatherApiService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<WeatherInfo> GetWeatherAsync(string city)
    {
        var uri = $"{BaseUrl}?q={city}&appid={ApiKey}&units=metric&lang=ru";
        var response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<WeatherInfo>();
        return data ?? throw new InvalidOperationException("Пустой ответ от API");
    }
}