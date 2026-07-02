using TodoApi.Clients;
using TodoApi.Clients.Interfaces;
using TodoApi.Data;
using TodoApi.Domain.Entities;
using TodoApi.Dtos;

namespace TodoApi.Providers;

public interface ITodoItemProvider
{
    Task<List<TodoItem>> GetItemsByListAsync(
        int listId,
        TodoStatus? status = null,
        bool overdueOnly = false);

    Task<TodoItem?> GetByIdAsync(int id);

    Task<List<TodoItemDto>> GetByListIdAsync(int listId, TodoStatus? status);

    Task<TodoItem> CreateAsync(int listId,
        string title,
        string? notes,
        Priority priority,
        DateTime? dueDate
    );

    Task UpdateStatusAsync(int itemId, TodoStatus status);


    Task DeleteAsync(int itemId);
}