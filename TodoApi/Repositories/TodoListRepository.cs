using Microsoft.EntityFrameworkCore;
using TodoApi.Data;

namespace TodoApi.Repositories;

public class TodoListRepository : ITodoListRepository
{
    private readonly AppDbContext _context;

    public TodoListRepository(AppDbContext context)
    {
        _context = context;
    }


    public Task<List<TodoList>> GetAllAsync()
        => _context.TodoLists.ToListAsync();

    public Task<TodoList?> GetByIdAsync(int id)
        => _context.TodoLists.FirstOrDefaultAsync(l => l.Id == id);
    
    public async Task<bool> CheckIfNameExists(string name)
    {
        if (_context.TodoLists.FirstOrDefaultAsync(l => l.Name == name) != null)
        {
            return true;
        }
        return false;
    }

    public async Task AddAsync(TodoList list)
        => await _context.TodoLists.AddAsync(list);

    public Task DeleteAsync(TodoList list)
    {
        _context.TodoLists.Remove(list);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}