using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using TaskManager.Commands;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IStorageService _storage;
    public ObservableCollection<TaskItem> Tasks { get; } = new();
    public ICollectionView TasksView { get; }
    public IEnumerable<TaskState> States => Enum.GetValues<TaskState>();
    public IEnumerable<PriorityLevel> Priorities => Enum.GetValues<PriorityLevel>();
    private TaskItem? _selectedTask;
    public TaskItem? SelectedTask
    {
        get => _selectedTask;
        set { _selectedTask = value; OnPropertyChanged(); }
    }
    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); TasksView.Refresh(); }
    }

    private TaskState? _filterState;
    public TaskState? FilterState
    {
        get => _filterState;
        set { _filterState = value; OnPropertyChanged(); TasksView.Refresh(); }
    }

    private PriorityLevel? _filterPriority;
    public PriorityLevel? FilterPriority
    {
        get => _filterPriority;
        set { _filterPriority = value; OnPropertyChanged(); TasksView.Refresh(); }
    }
    private string _sortBy = "Date";
    public string SortBy
    {
        get => _sortBy;
        set { _sortBy = value; OnPropertyChanged(); ApplySorting(); }
    }
    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }

    public MainViewModel(IStorageService storage)
    {
        _storage = storage;
        TasksView = CollectionViewSource.GetDefaultView(Tasks);
        TasksView.Filter = FilterPredicate;
        AddCommand = new RelayCommand(AddTask);
        DeleteCommand = new RelayCommand(DeleteTask, () => SelectedTask != null);
        SaveCommand = new RelayCommand(SaveTasks);
        LoadCommand = new RelayCommand(LoadTasks);
        LoadTasks();
    }

    private void AddTask()
    {
        var newTask = new TaskItem { Title = "Новая задача", CreatedDate = DateTime.Now };
        Tasks.Add(newTask);
        SelectedTask = newTask;
    }

    private void DeleteTask()
    {
        if (SelectedTask != null)
        {
            Tasks.Remove(SelectedTask);
            SelectedTask = null;
        }
    }

    private void SaveTasks() => _storage.Save(Tasks.ToList());
    
    private void LoadTasks()
    {
        var loaded = _storage.Load();
        Tasks.Clear();
        foreach (var task in loaded) Tasks.Add(task);
    }
    private bool FilterPredicate(object obj)
    {
        if (obj is not TaskItem task) return false;
        
        var matchesSearch = string.IsNullOrWhiteSpace(SearchText) ||
            task.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            task.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        
        var matchesState = FilterState == null || task.State == FilterState;
        var matchesPriority = FilterPriority == null || task.Priority == FilterPriority;
        
        return matchesSearch && matchesState && matchesPriority;
    }
    private void ApplySorting()
    {
        TasksView.SortDescriptions.Clear();
        
        if (SortBy == "Date")
            TasksView.SortDescriptions.Add(
                new SortDescription(nameof(TaskItem.CreatedDate), ListSortDirection.Descending));
        else
            TasksView.SortDescriptions.Add(
                new SortDescription(nameof(TaskItem.Title), ListSortDirection.Ascending));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}