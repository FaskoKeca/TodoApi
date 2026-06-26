using TodoApi.Data;
using TodoApi.Repositories;
using System.Linq;

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

    public async Task<TodoItem> AssignTagsAsync(int itemId, List<string> tagNames)
    {
        var item = await itemRepo.GetByIdAsync(itemId)
                   ?? throw new KeyNotFoundException("Item not found.");

        foreach (var raw in tagNames)
        {
            var name = raw.Trim().ToLower();

            var tag = await tagRepo.GetByNameAsync(name);

            if (tag == null)
            {
                tag = new Tag
                {
                    Name = name
                };

                await tagRepo.AddAsync(tag);
                await tagRepo.SaveChangesAsync();
            }

            bool alreadyLinked = item.TodoItemTags
                .Any(x => x.TagId == tag.Id);

            if (alreadyLinked)
                continue;

            item.TodoItemTags.Add(new TodoItemTag
            {
                TodoItemId = item.Id,
                TagId = tag.Id
            });
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