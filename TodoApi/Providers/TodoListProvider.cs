using TodoApi.Data;
using TodoApi.Domain.Entities;
using TodoApi.Repositories;

namespace TodoApi.Providers;

public class TodoListProvider(ITodoListRepository repo) : ITodoListProvider
{
    public Task<List<TodoList>> GetAllAsync()
        => repo.GetAllAsync();

    public Task<TodoList?> GetByIdAsync(int id)
        => repo.GetByIdAsync(id);

    public async Task<TodoList> CreateAsync(string name, string? description)
    {
        var existing = await repo.GetAllAsync();
        if (existing.Any(x => x.Name == name))
            throw new Exception($"TodoList with name {name} already exists");

        var list = new TodoList
        {
            Name = name,
            Description = description,
            Created = DateTime.UtcNow,
            IsArchived = false
        };

        await repo.AddAsync(list);
        await repo.SaveChangesAsync();

        return list;
    }

    public async Task ArchiveAsync(int id)
    {
        var list = await repo.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException("List not found.");

        if (list.Items.Any(i => i.Status != TodoStatus.Completed))
            throw new InvalidOperationException("Cannot archive list with open items.");

        list.IsArchived = true;
        
        await repo.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var list = await repo.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException("List not found.");
        
        if(list.Items.Any(i => i.Status != TodoStatus.Completed))
            throw new InvalidOperationException("Cannot delete list with open items.");
        
        if(list.IsArchived == false)
            throw new InvalidOperationException("Cannot delete an unarchived list.");
        
        await repo.DeleteAsync(list);
        await repo.SaveChangesAsync();
        return true;
    }
}