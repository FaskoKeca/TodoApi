using TodoApi.Data;

namespace TodoApi.Dtos;

public class TodoListDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
}