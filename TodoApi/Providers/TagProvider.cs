using TodoApi.Data;
using TodoApi.Repositories;

namespace TodoApi.Providers;

public class TagProvider(ITagRepository repo) : ITagProvider
{
    public Task<List<Tag>> GetAllAsync()
        => repo.GetAllAsync();

    public async Task<Tag> CreateAsync(string name)
    {
        name = name.Trim().ToLower();
        var existing = await repo.GetByNameAsync(name);
        if (existing != null)
            throw new InvalidOperationException("TodoList with name already exists");

        var tag = new Tag
        {
            Name = name,
        };

        await repo.AddAsync(tag);
        await repo.SaveChangesAsync();

        return tag;
    }

    public async Task<int> MergeAsync(int sourceTagId, int targetTagId)
    {
        if (sourceTagId == targetTagId)
            throw new InvalidOperationException("Cannot merge a tag into itself.");

        var source = await repo.GetByIdAsync(sourceTagId)
                     ?? throw new KeyNotFoundException("Source tag not found.");

        var target = await repo.GetByIdAsync(targetTagId)
                     ?? throw new KeyNotFoundException("Target tag not found.");

        int affectedItems = 0;

        foreach (var itemTag in source.TodoItemTags.ToList())
        {
            bool alreadyAssigned = target.TodoItemTags
                .Any(t => t.TodoItemId == itemTag.TodoItemId);

            if (!alreadyAssigned)
            {
                target.TodoItemTags.Add(new TodoItemTag
                {
                    TodoItemId = itemTag.TodoItemId,
                    TagId = target.Id
                });

                affectedItems++;
            }
        }
        await repo.DeleteAsync(source);
        await repo.SaveChangesAsync();

        return affectedItems;
    }

    public async Task<int> DeleteAsync(int tagId)
    {
        var tag = await repo.GetByIdAsync(tagId)
                  ?? throw new KeyNotFoundException("Tag not found.");

        int affectedItems = tag.TodoItemTags.Count;

        await repo.DeleteAsync(tag);
        await repo.SaveChangesAsync();

        return affectedItems;
    }
}