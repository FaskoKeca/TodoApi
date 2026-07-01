using TodoApi.Data;
using TodoApi.Domain.Entities;

namespace TodoApi.Dtos;

public class TagDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<TodoItemTagsDto> TodoItemTags { get; set; } = new List<TodoItemTagsDto>();
}