using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Domain.Entities;

namespace TodoApi.Repositories;

public class TodoListRepository(AppDbContext context) : ITodoListRepository
{
    public async Task<List<TodoList>> GetAllAsync()
        => await context.TodoLists.AsNoTracking().ToListAsync();

    public async Task<TodoList?> GetByIdAsync(int id)
        => await context.TodoLists.FirstOrDefaultAsync(l => l.Id == id);

    public async Task<bool> CheckIfNameExists(string name)
        => await context.TodoLists.FirstOrDefaultAsync(l => l.Name == name) != null;


    public async Task AddAsync(TodoList list)
        => await context.TodoLists.AddAsync(list);

    public void Delete(TodoList list)
        => context.TodoLists.Remove(list);
    

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();
}