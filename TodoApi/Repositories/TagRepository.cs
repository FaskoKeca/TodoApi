using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Domain.Entities;

namespace TodoApi.Repositories;

public class TagRepository(AppDbContext context) : ITagRepository
{
    public async Task<List<Tag>> GetAllAsync()
        => await context.Tags
            .AsNoTracking()
            .Include(t => t.TodoItemTags)
            .ThenInclude(tt => tt.TodoItem)
            .ToListAsync();

    public async Task<Tag?> GetByIdAsync(int id)
        => await context.Tags
            .Include(t => t.TodoItemTags)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Tag?> GetByNameAsync(string name)
        => await context.Tags
            .FirstOrDefaultAsync(t => t.Name == name);

    public async Task AddAsync(Tag tag)
        => await context.Tags.AddAsync(tag);
    


    public void Delete(Tag tag)
    {
        context.Tags.Remove(tag);
    }

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();
}