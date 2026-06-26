namespace TodoApi.Data;

public class TodoItemTag
{
    public int TodoItemId { get; set; }
    
    public int TagId { get; set; }

    
    
    public TodoItem TodoItem { get; set; } = null!;
    
    public Tag Tag { get; set; } = null!;
}
