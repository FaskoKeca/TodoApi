using System.ComponentModel.DataAnnotations;

namespace TodoApi.Domain.Entities;

public class TodoList
{
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public DateTime Created { get; set; }

        public bool IsArchived { get; set; }

        public ICollection<TodoItem> Items { get; set; } = new List<TodoItem>();
}