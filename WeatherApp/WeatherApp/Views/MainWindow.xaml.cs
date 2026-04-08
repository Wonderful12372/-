using System.Net.Http;
using System.Windows;
using WeatherApp.Services;
using WeatherApp.ViewModels;

namespace WeatherApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        var httpClient = new HttpClient();
        var weatherService = new WeatherApiService(httpClient);
        var storageService = new JsonStorageService();
        
        var viewModel = new MainWindowViewModel(weatherService, storageService);
        DataContext = viewModel;
        
        _ = viewModel.InitializeAsync();
    }
}