using System.ComponentModel.DataAnnotations;

namespace TodoApi.Domain.Entities;

public class Tag
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<TodoItemTag> TodoItemTags { get; set; } = new List<TodoItemTag>();
}