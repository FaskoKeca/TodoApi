using TodoApi.Data;

namespace TodoApi.Repositories;

public interface ITodoItemRepository
{
    Task<TodoItem?> GetByIdAsync(int id);
    Task<TodoItem?> GetByListIdAsync(int id);
    Task<TodoItem?> GetByStatusAsync(TodoStatus status);
    Task AddAsync(TodoItem item);
    Task DeleteAsync(TodoItem item);
    Task UpdateAsync(TodoItem item);
    Task SaveChangesAsync();
}