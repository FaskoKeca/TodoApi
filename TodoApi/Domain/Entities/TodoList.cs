namespace TodoApi.Domain.Entities;

public class TodoList
{
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime Created { get; set; }

        public bool IsArchived { get; set; }

        public ICollection<TodoItem> Items { get; set; } = new List<TodoItem>();
}