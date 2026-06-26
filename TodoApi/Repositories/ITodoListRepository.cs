using TodoApi.Data;

namespace TodoApi.Repositories;

public interface ITodoListRepository
{
    Task<List<TodoList>> GetAllAsync();
    Task<TodoList?> GetByIdAsync(int id);
    Task AddAsync(TodoList list);
    Task DeleteAsync(TodoList list);
    Task SaveChangesAsync();
    Task<bool> CheckIfNameExists(string name);
}