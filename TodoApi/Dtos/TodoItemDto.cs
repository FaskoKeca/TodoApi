using TodoApi.Data;
using TodoApi.Domain.Entities;

namespace TodoApi.Dtos;

public class TodoItemDto
{
    public int Id { get; set; }

    public int TodoListId { get; set; }
    

    public string Title { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public Priority Priority { get; set; }

    public TodoStatus Status { get; set; }

    public DateTime? Due { get; set; }

    public List<TodoItemTagsDto> TodoItemTags { get; set; } = new List<TodoItemTagsDto>();
}