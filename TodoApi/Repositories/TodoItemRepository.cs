using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Domain.Entities;

namespace TodoApi.Repositories;

public class TodoItemRepository(AppDbContext context) : ITodoItemRepository
{
    public async Task<TodoItem?> GetByIdAsync(int id)
        => await context.TodoItems
            .Include(x => x.TodoItemTags)
            .FirstOrDefaultAsync(x => x.Id == id);
    
    public async Task<List<TodoItem>> GetByListIdAsync(int id)
        => await context.TodoItems
            .Where(x => x.TodoListId == id)
            .ToListAsync();
    
    public async Task<TodoItem?> GetByStatusAsync(TodoStatus status)
        => await context.TodoItems.FirstOrDefaultAsync(i => i.Status == status);
    

    public async Task AddAsync(TodoItem item)
        => await context.TodoItems.AddAsync(item);
    
    public async Task AddItemTagAsync(int itemId, int tagId)
    {
        await context.TodoItemTags.AddAsync(new TodoItemTag
        {
            TodoItemId = itemId,
            TagId = tagId
        });
    }
    

    public void Delete(TodoItem item)
    {
        context.TodoItems.Remove(item);
    }
    
    public async Task AddTagAsync(int itemId, int tagId)
    {
        await context.TodoItemTags.AddAsync(new TodoItemTag
        {
            TodoItemId = itemId,
            TagId = tagId
        });
    }

    public async Task<bool> ItemTagExistsAsync(int itemId, int tagId)
    {
        return await context.TodoItemTags
            .AnyAsync(x => x.TodoItemId == itemId && x.TagId == tagId);
    }

    public void Update(TodoItem item)
    {
        context.TodoItems.Update(item);
    }

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();
}