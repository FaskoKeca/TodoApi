using TodoApi.Data;

namespace TodoApi.Providers;

public interface ITodoItemProvider
{
    Task<List<TodoItem>> GetItemsByListAsync(
        int listId,
        TodoStatus? status = null,
        bool overdueOnly = false);

    Task<TodoItem?> GetByIdAsync(int id);

    Task<TodoItem> CreateAsync(
        int listId,
        string title,
        string? notes,
        Priority priority,
        DateTime? dueDate);

    Task UpdateStatusAsync(int itemId, TodoStatus status);

    Task<TodoItem> CompleteAsync(int itemId);

    Task<TodoItem> AssignTagsAsync(int itemId, List<string> tagNames);

    Task DeleteAsync(int itemId);
}