using Microsoft.EntityFrameworkCore;
using TodoApi.Data;

namespace TodoApi.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _context;

    public TagRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public Task<List<Tag>> GetAllAsync()
        => _context.Tags
            .Include(t => t.TodoItemTags)
            .ThenInclude(tt => tt.TodoItem)
            .ToListAsync();

    public Task<Tag?> GetByIdAsync(int id)
        => _context.Tags
            .Include(t => t.TodoItemTags)
            .FirstOrDefaultAsync(t => t.Id == id);

    public Task<Tag?> GetByNameAsync(string name)
        => _context.Tags
            .FirstOrDefaultAsync(t => t.Name == name);

    public async Task AddAsync(Tag tag)
        => await _context.Tags.AddAsync(tag);

    public Task DeleteAsync(Tag tag)
    {
        _context.Tags.Remove(tag);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}