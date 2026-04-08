using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IWeatherService _weatherService;
    private readonly IDataStorageService _storageService;

    [ObservableProperty] private string _cityName = string.Empty;
    [ObservableProperty] private WeatherInfo? _currentWeather;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _iconUrl = string.Empty;
    [ObservableProperty] private string _theme = "Light";
    [ObservableProperty] private ObservableCollection<string> _history = new();

    public MainWindowViewModel(IWeatherService weatherService, IDataStorageService storageService)
    {
        _weatherService = weatherService;
        _storageService = storageService;
    }

    public async Task InitializeAsync()
    {
        Theme = await _storageService.LoadThemeAsync();
        ApplyTheme(Theme);
        
        CityName = await _storageService.LoadLastCityAsync();
        History = new ObservableCollection<string>(await _storageService.LoadHistoryAsync());

        if (!string.IsNullOrWhiteSpace(CityName))
            await SearchWeatherAsync();
    }

    private void ApplyTheme(string theme)
    {
        if (Application.Current is App app)
        {
            app.SetTheme(theme);
        }
    }

    partial void OnThemeChanged(string value)
    {
        ApplyTheme(value);
        _ = _storageService.SaveThemeAsync(value);
    }

    [RelayCommand]
    private async Task SearchWeatherAsync()
    {
        if (string.IsNullOrWhiteSpace(CityName.Trim())) return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        CurrentWeather = null;
        IconUrl = string.Empty;

        try
        {
            CurrentWeather = await _weatherService.GetWeatherAsync(CityName.Trim());
            IconUrl = $"https://openweathermap.org/img/wn/{CurrentWeather.Weather[0].Icon}@2x.png";
            await SaveToHistoryAsync();
            await _storageService.SaveLastCityAsync(CityName);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"❌ Ошибка: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void ChangeTheme()
    {
        Theme = Theme == "Light" ? "Dark" : "Light";
    }

    private async Task SaveToHistoryAsync()
    {
        var city = CityName.Trim();
        if (!History.Contains(city, StringComparer.OrdinalIgnoreCase))
        {
            if (History.Count >= 5) History.RemoveAt(0);
            History.Add(city);
            await _storageService.SaveHistoryAsync(History.ToList());
        }
    }

    [RelayCommand]
    private async Task SelectFromHistoryAsync(string city)
    {
        CityName = city;
        await SearchWeatherAsync();
    }
}