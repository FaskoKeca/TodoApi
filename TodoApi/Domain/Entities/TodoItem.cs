namespace TodoApi.Data;

public class TodoItem
{
    public int Id { get; set; }

    public int TodoListId { get; set; }

    public TodoList TodoList { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public Priority Priority { get; set; }

    public TodoStatus Status { get; set; }

    public DateTime? Due { get; set; }

    public DateTime? Completed { get; set; }

    public DateTime CreatedUtc { get; set; }

    public ICollection<TodoItemTag> TodoItemTags { get; set; } = new List<TodoItemTag>();
}