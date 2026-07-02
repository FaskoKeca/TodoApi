using TodoApi.Domain.Entities;

namespace TodoApi.Repositories.Interfaces;

public interface ITagRepository
{
    Task<List<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task<Tag?> GetByNameAsync(string name);

    Task AddAsync(Tag tag);
    void Delete(Tag tag);
    


    Task SaveChangesAsync();
}