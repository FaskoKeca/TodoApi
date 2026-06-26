namespace TodoApi.Data;

public class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<TodoItemTag> TodoItemTags { get; set; } = new List<TodoItemTag>();
}