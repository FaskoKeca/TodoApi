using TodoApi.Data;
using TodoApi.Domain.Entities;

namespace TodoApi.Repositories;

public interface ITodoListRepository
{
    Task<List<TodoList>> GetAllAsync();
    Task<TodoList?> GetByIdAsync(int id);
    Task AddAsync(TodoList list);
    void Delete(TodoList list);
    Task SaveChangesAsync();
    Task<bool> CheckIfNameExists(string name);
}