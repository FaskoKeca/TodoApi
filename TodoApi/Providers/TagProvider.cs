using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Domain.Entities;
using TodoApi.Repositories;

namespace TodoApi.Providers;

public class TagProvider(
    ITodoItemRepository itemRepo,
    ITagRepository tagRepo) : ITagProvider
{
    public Task<List<Tag>> GetAllAsync()
        => tagRepo.GetAllAsync();

    public async Task<Tag> CreateAsync(string name)
    {
        name = name.Trim().ToLower();
        var existing = await tagRepo.GetByNameAsync(name);
        if (existing != null)
            throw new InvalidOperationException("TodoList with name already exists");

        var tag = new Tag
        {
            Name = name,
        };

        await tagRepo.AddAsync(tag);
        await tagRepo.SaveChangesAsync();

        return tag;
    }
    
    public async Task AssignToItemAsync(int itemId, List<int> tagIds)
    {
        var item = await itemRepo.GetByIdAsync(itemId)
                   ?? throw new KeyNotFoundException("Todo item not found.");

        foreach (var tagId in tagIds)
        {
            var tag = await tagRepo.GetByIdAsync(tagId)
                      ?? throw new KeyNotFoundException($"Tag {tagId} not found.");

            var exists = await itemRepo.ItemTagExistsAsync(itemId, tagId);

            if (exists)
                continue;

            await itemRepo.AddItemTagAsync(itemId, tagId);
        }

        await itemRepo.SaveChangesAsync();
    }

    public async Task<Tag> GetByIdAsync(int id)
    {
        var tag = await tagRepo.GetByIdAsync(id);
        return tag ?? throw new KeyNotFoundException("Tag not found.");
    }

    public async Task<int> MergeAsync(int sourceTagId, int targetTagId)
    {
        if (sourceTagId == targetTagId)
            throw new InvalidOperationException("Cannot merge a tag into itself.");

        var source = await tagRepo.GetByIdAsync(sourceTagId)
                     ?? throw new KeyNotFoundException("Source tag not found.");

        var target = await tagRepo.GetByIdAsync(targetTagId)
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
        tagRepo.Delete(source);
        await tagRepo.SaveChangesAsync();

        return affectedItems;
    }

    public async Task<int> DeleteAsync(int tagId)
    {
        var tag = await tagRepo.GetByIdAsync(tagId)
                  ?? throw new KeyNotFoundException("Tag not found.");

        int affectedItems = tag.TodoItemTags.Count;

        tagRepo.Delete(tag);
        await tagRepo.SaveChangesAsync();

        return affectedItems;
    }
}