using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class WeatherInfo
{
    [JsonPropertyName("name")] public string CityName { get; set; } = string.Empty;
    [JsonPropertyName("main")] public MainData Main { get; set; } = new();
    [JsonPropertyName("weather")] public List<WeatherDescription> Weather { get; set; } = new();
    [JsonPropertyName("wind")] public WindData Wind { get; set; } = new();
}

public class MainData
{
    [JsonPropertyName("temp")] public double Temperature { get; set; }
    [JsonPropertyName("humidity")] public int Humidity { get; set; }
    [JsonPropertyName("pressure")] public int Pressure { get; set; }
}

public class WeatherDescription
{
    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
    [JsonPropertyName("icon")] public string Icon { get; set; } = string.Empty;
}

public class WindData
{
    [JsonPropertyName("speed")] public double Speed { get; set; }
}