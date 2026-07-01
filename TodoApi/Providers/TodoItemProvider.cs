using TodoApi.Data;
using TodoApi.Repositories;
using System.Linq;
using TodoApi.Common.Exceptions;
using TodoApi.Domain.Entities;
using TodoApi.Dtos;

namespace TodoApi.Providers;

public class TodoItemProvider(ITodoListRepository listRepo, ITodoItemRepository itemRepo, ITagRepository tagRepo)
    : ITodoItemProvider
{
    public async Task<List<TodoItem>> GetItemsByListAsync(
        int listId,
        TodoStatus? status = null,
        bool overdueOnly = false)
    {
        var list = await listRepo.GetByIdAsync(listId)
                   ?? throw new KeyNotFoundException("List not found.");

        var items = await itemRepo.GetByListIdAsync(listId);

        if (status.HasValue)
        {
            items = items
                .Where(i => i.Status == status.Value)
                .ToList();
        }

        if (overdueOnly)
        {
            items = items
                .Where(i =>
                    i.Due.HasValue &&
                    i.Due.Value < DateTime.Now &&
                    i.Status != TodoStatus.Completed)
                .ToList();
        }

        return items;
    }

    public Task<TodoItem?> GetByIdAsync(int id)
        => itemRepo.GetByIdAsync(id);

    public async Task<List<TodoItemDto>> GetByListIdAsync(int listId, TodoStatus? status)
    {
        var list = await listRepo.GetByIdAsync(listId)
                   ?? throw new NotFoundException("List not found");

        var items = await itemRepo.GetByListIdAsync(listId);

        if (status.HasValue)
            items = items.Where(x => x.Status == status.Value).ToList();

        return items.Select(x => new TodoItemDto
        {
            Id = x.Id,
            TodoListId = x.TodoListId,
            Title = x.Title,
            Notes = x.Notes,
            Priority = x.Priority,
            Due = x.Due,
            TodoItemTags = x.TodoItemTags
        }).ToList();
    }
    
    public async Task<TodoItem> CreateAsync(
        int listId,
        string title,
        string? notes,
        Priority priority,
        DateTime? dueDate)
    {
        var list = await listRepo.GetByIdAsync(listId)
                   ?? throw new KeyNotFoundException("List not found.");

        if (list.IsArchived)
            throw new InvalidOperationException("Cannot add items to archived list.");

        if (dueDate.HasValue && dueDate.Value < DateTime.Now)
            throw new InvalidOperationException("Due date cannot be in the past.");

        var item = new TodoItem
        {
            TodoListId = listId,
            Title = title,
            Notes = notes,
            Priority = priority,
            Status = TodoStatus.Pending,
            Due = dueDate,
            Created = DateTime.Now
        };

        await itemRepo.AddAsync(item);
        await itemRepo.SaveChangesAsync();

        return item;
    }

    public async Task UpdateStatusAsync(int itemId, TodoStatus status)
    {
        var item = await itemRepo.GetByIdAsync(itemId)
                   ?? throw new KeyNotFoundException("Item not found.");

        if (item.Status == TodoStatus.Completed &&
            status == TodoStatus.Pending)
        {
            throw new InvalidOperationException("Cannot move Completed → Pending.");
        }

        item.Status = status;

        if (status == TodoStatus.Completed)
            item.Completed = DateTime.Now;

        await itemRepo.SaveChangesAsync();
    }

    public async Task<TodoItem> CompleteAsync(int itemId)
    {
        var item = await itemRepo.GetByIdAsync(itemId)
                   ?? throw new KeyNotFoundException("Item not found.");

        if (item.Status == TodoStatus.Completed)
            throw new InvalidOperationException("Item already completed.");

        item.Status = TodoStatus.Completed;
        item.Completed = DateTime.Now;

        await itemRepo.SaveChangesAsync();

        return item;
    }

    public async Task<TodoItem> AssignTagsAsync(int itemId, List<int> tagIds)
    {
        var item = await itemRepo.GetByIdAsync(itemId)
                   ?? throw new KeyNotFoundException("Item not found.");

        foreach (var tagId in tagIds)
        {
            var tag = await tagRepo.GetByIdAsync(tagId);

            if (tag == null)
                throw new KeyNotFoundException($"Tag {tagId} not found.");

            var exists = await itemRepo.ItemTagExistsAsync(itemId, tagId);
            if (exists)
                continue;

            await itemRepo.AddTagAsync(itemId, tagId);
        }

        await itemRepo.SaveChangesAsync();

        return item;
    }

    public async Task DeleteAsync(int itemId)
    {
        var item = await itemRepo.GetByIdAsync(itemId)
                   ?? throw new KeyNotFoundException("Item not found.");

        await itemRepo.DeleteAsync(item);
        await itemRepo.SaveChangesAsync();
    }
}