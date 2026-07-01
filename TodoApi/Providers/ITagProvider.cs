using TodoApi.Data;
using TodoApi.Domain.Entities;

namespace TodoApi.Providers;

public interface ITagProvider
{
    Task<List<Tag>> GetAllAsync();

    Task<Tag> CreateAsync(string name);

    Task<int> MergeAsync(int sourceTagId, int targetTagId);

    Task<int> DeleteAsync(int tagId);
    
    Task AssignToItemAsync(int itemId, List<int> tagIds);
}