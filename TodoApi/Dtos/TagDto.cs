using TodoApi.Data;

namespace TodoApi.Dtos;

public class TagDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<TodoItemTag> TodoItemTags { get; set; } = new List<TodoItemTag>();
}