using Microsoft.EntityFrameworkCore;
using TodoApi.Data;

namespace TodoApi.Repositories;

public class TodoItemRepository : ITodoItemRepository
{
    private readonly AppDbContext _context;

    public TodoItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<TodoItem?> GetByIdAsync(int id)
        => _context.TodoItems.FirstOrDefaultAsync(l => l.Id == id);
    
    public async Task<List<TodoItem>> GetByListIdAsync(int id)
        => await _context.TodoItems
            .Where(x => x.TodoListId == id)
            .ToListAsync();
    
    public async Task<TodoItem?> GetByStatusAsync(TodoStatus status)
        => await _context.TodoItems.FirstOrDefaultAsync(i => i.Status == status);
    

    public async Task AddAsync(TodoItem item)
        => await _context.TodoItems.AddAsync(item);

    public Task DeleteAsync(TodoItem item)
    {
        _context.TodoItems.Remove(item);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TodoItem item)
    {
        _context.TodoItems.Update(item);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}