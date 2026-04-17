using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TaskManager.Models;

namespace TaskManager.Services;

public interface IStorageService
{
    List<TaskItem> Load();
    void Save(List<TaskItem> tasks);
}

public class JsonStorageService : IStorageService
{
    private readonly string _filePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TaskManager", "tasks.json");

    public JsonStorageService()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
    }

    public List<TaskItem> Load()
    {
        if (!File.Exists(_filePath)) return new List<TaskItem>();
        
        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
        }
        catch
        {
            return new List<TaskItem>();
        }
    }

    public void Save(List<TaskItem> tasks)
    {
        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true, 
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };
        
        var json = JsonSerializer.Serialize(tasks, options);
        File.WriteAllText(_filePath, json);
    }
}