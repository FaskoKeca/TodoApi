using TodoApi.Domain.Entities;

namespace TodoApi.Providers.Interfaces;

public interface ITodoListProvider
{
        Task<List<TodoList>> GetAllAsync();
        Task<TodoList?> GetByIdAsync(int id);

        Task<TodoList> CreateAsync(string name, string? description);
        Task ArchiveAsync(int id);
        Task<bool> DeleteAsync(int id);
}