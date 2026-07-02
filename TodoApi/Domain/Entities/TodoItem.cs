using System.ComponentModel.DataAnnotations;

namespace TodoApi.Domain.Entities;

public class TodoItem
{
    public int Id { get; set; }

    public int TodoListId { get; set; }

    public TodoList TodoList { get; set; } = null!;

    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Notes { get; set; }

    public Priority Priority { get; set; }

    public TodoStatus Status { get; set; }

    public DateTime? Due { get; set; }

    public DateTime? Completed { get; set; }

    public DateTime Created { get; set; }

    public ICollection<TodoItemTag> TodoItemTags { get; set; } = new List<TodoItemTag>();
}