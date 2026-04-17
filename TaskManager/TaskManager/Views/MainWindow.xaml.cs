using System.Windows;
using TaskManager.Services;
using TaskManager.ViewModels;

namespace TaskManager.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(new JsonStorageService());
    }
}