using Microsoft.EntityFrameworkCore;

namespace TodoApi.Data;

public class AppDbContext : DbContext
{
    DbSet<TodoList> TodoLists { get; set; }
    DbSet<TodoItem> TodoItems { get; set; }
    DbSet<Tag> Tags { get; set; }
}