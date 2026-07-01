using TodoApi.Data;
using TodoApi.Domain.Entities;

namespace TodoApi.Repositories;

public interface ITodoItemRepository
{
    Task<TodoItem?> GetByIdAsync(int id);
    Task<List<TodoItem>> GetByListIdAsync(int listId);
    Task<TodoItem?> GetByStatusAsync(TodoStatus status);
    Task AddAsync(TodoItem item);
    Task DeleteAsync(TodoItem item);
    Task UpdateAsync(TodoItem item);
    Task SaveChangesAsync();
    Task AddTagAsync(int itemId, int tagId);
    Task<bool> ItemTagExistsAsync(int itemId, int tagId);
}