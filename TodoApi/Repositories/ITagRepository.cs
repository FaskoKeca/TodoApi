using TodoApi.Data;

namespace TodoApi.Repositories;

public interface ITagRepository
{
    Task<List<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task<Tag?> GetByNameAsync(string name);

    Task AddAsync(Tag tag);
    Task DeleteAsync(Tag tag);

    Task SaveChangesAsync();
}