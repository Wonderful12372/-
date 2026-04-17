using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TaskManager.Models;
public enum TaskState { Новая, В_процессе, Выполнена, Отменена }
public enum PriorityLevel { Низкий, Средний, Высокий }

public class TaskItem : INotifyPropertyChanged, INotifyDataErrorInfo
{
    private string _title = string.Empty;
    
    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged();
                Validate(nameof(Title));
            }
        }
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set { if (_description != value) { _description = value; OnPropertyChanged(); } }
    }

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    private TaskState _state = TaskState.Новая;
    public TaskState State
    {
        get => _state;
        set { if (_state != value) { _state = value; OnPropertyChanged(); } }
    }
    
    private PriorityLevel _priority = PriorityLevel.Средний;
    public PriorityLevel Priority
    {
        get => _priority;
        set { if (_priority != value) { _priority = value; OnPropertyChanged(); } }
    }
    private readonly Dictionary<string, List<string>> _errors = new();
    public bool HasErrors => _errors.Any();
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName) =>
        propertyName != null && _errors.TryGetValue(propertyName, out var list) 
            ? list 
            : Enumerable.Empty<string>();

    private void Validate(string propertyName)
    {
        _errors.Remove(propertyName);
        
        if (propertyName == nameof(Title) && string.IsNullOrWhiteSpace(Title))
            _errors[propertyName] = ["Название задачи не может быть пустым."];
        
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}