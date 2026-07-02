using TodoApi.Clients.Interfaces;
using TodoApi.Common.Exceptions;
using TodoApi.Domain.Entities;
using TodoApi.Dtos;
using TodoApi.Repositories.Interfaces;

namespace TodoApi.Providers;

public class TodoItemProvider(
    ITodoListRepository listRepo,
    ITodoItemRepository itemRepo,
    ISchedulerClient schedulerClient,
    INotificationClient notificationClient,
    ILogger<TodoItemProvider> logger)
    : ITodoItemProvider
{
    public async Task<List<TodoItem>> GetItemsByListAsync(
        int listId,
        TodoStatus? status = null,
        bool overdueOnly = false)
    {
        if (await listRepo.GetByIdAsync(listId) is null)
            throw new KeyNotFoundException("List not found.");

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
        var items = await itemRepo.GetByListIdAsync(listId)
                    ?? throw new NotFoundException("List not found");

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
                .Select(t => new TodoItemTagsDto
                {
                    TodoItemId = t.TodoItemId,
                    TagId = t.TagId,
                    Tag = t.Tag
                })
                .ToList()
        }).ToList();
    }


    public async Task<TodoItem> CreateAsync(int listId,
        string title,
        string? notes,
        Priority priority,
        DateTime? dueDate
    )
    {
        var list = await listRepo.GetByIdAsync(listId)
                   ?? throw new KeyNotFoundException("List not found.");

        if (list.IsArchived)
            throw new InvalidOperationException("Cannot add items to archived list.");

        if (dueDate.HasValue && dueDate.Value < DateTime.Now)
            throw new InvalidOperationException("Due date cannot be in the past.");

        var ct = CancellationToken.None;
        if (dueDate.HasValue)
        {
            var check = await schedulerClient.IsHolidayAsync(dueDate.Value, ct);

            if (check?.IsHoliday == true)
                throw new BusinessRuleException(
                    $"Due date falls on a holiday: {check.Name}");
        }

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
        {
            item.Completed = DateTime.Now;

            await itemRepo.SaveChangesAsync(); // IMPORTANT: persist first

            try
            {
                await notificationClient.SendAsync(new SendNotificationRequest
                {
                    Channel = "email",
                    Recipient = "user@test.com", // placeholder for now
                    Subject = "Todo completed",
                    Message = $"Todo '{item.Title}' was completed.",
                    Reference = "go6o"
                }, CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Notification failed for item {ItemId}", item.Id);
            }

            return;
        }

        await itemRepo.SaveChangesAsync();
    }


    public async Task DeleteAsync(int itemId)
    {
        var item = await itemRepo.GetByIdAsync(itemId)
                   ?? throw new KeyNotFoundException("Item not found.");

        itemRepo.Delete(item);
        await itemRepo.SaveChangesAsync();
    }
}