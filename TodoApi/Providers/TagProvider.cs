using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Domain.Entities;
using TodoApi.Repositories;

namespace TodoApi.Providers;

public class TagProvider : ITagProvider
{
    private readonly ITodoItemRepository _itemRepo;
    private readonly ITagRepository repo;

    public TagProvider(
        ITodoItemRepository itemRepo,
        ITagRepository tagRepo)
    {
        _itemRepo = itemRepo;
        repo = tagRepo;
    }

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
    
    public async Task AssignToItemAsync(int itemId, List<int> tagIds)
    {
        var item = await _itemRepo.GetByIdAsync(itemId)
                   ?? throw new KeyNotFoundException("Todo item not found.");

        foreach (var tagId in tagIds)
        {
            var tag = await repo.GetByIdAsync(tagId)
                      ?? throw new KeyNotFoundException($"Tag {tagId} not found.");

            var exists = await _itemRepo.ItemTagExistsAsync(itemId, tagId);

            if (exists)
                continue;

            await _itemRepo.AddItemTagAsync(itemId, tagId);
        }

        await _itemRepo.SaveChangesAsync();
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