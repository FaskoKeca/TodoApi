using TodoApi.Data;
using TodoApi.Repositories;

namespace TodoApi.Providers;

public class TodoListProvider : ITodoListProvider
{
    private readonly ITodoListRepository _repo;

    public TodoListProvider(ITodoListRepository repo)
    {
        _repo = repo;
    }


    public Task<List<TodoList>> GetAllAsync()
        => _repo.GetAllAsync();

    public Task<TodoList?> GetByIdAsync(int id)
        => _repo.GetByIdAsync(id);

    public async Task<TodoList> CreateAsync(string name, string? description)
    {
        var existing = await _repo.GetAllAsync();
        if (existing.Any(x => x.Name == name))
            throw new Exception($"TodoList with name {name} already exists");

        var list = new TodoList
        {
            Name = name,
            Description = description,
            Created = DateTime.UtcNow,
            IsArchived = false
        };

        await _repo.AddAsync(list);
        await _repo.SaveChangesAsync();

        return list;
    }

    public async Task ArchiveAsync(int id)
    {
        var list = await _repo.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException("List not found.");

        if (list.Items.Any(i => i.Status != TodoStatus.Completed))
            throw new InvalidOperationException("Cannot archive list with open items.");

        list.IsArchived = true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var list = await _repo.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException("List not found.");
        
        if(list.Items.Any(i => i.Status != TodoStatus.Completed))
            throw new InvalidOperationException("Cannot delete list with open items.");
        
        await _repo.DeleteAsync(list);
        await _repo.SaveChangesAsync();
        return true;
    }
}