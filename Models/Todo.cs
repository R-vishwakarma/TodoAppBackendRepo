
using TodoApp.Enums;
namespace TodoApp.Models


{
    public class Todo
    {
        public int Id { get; set; } // Primary Key
        public string Title { get; set; }
        public string Description { get; set; }
        public PriorityLevel Priority { get; set; } // Enum for Priority
        public TodoCategory Category { get; set; } // Enum for Category
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } // Timestamp when the todo was created

        // Foreign Key
        public int UserId { get; set; }
        public User User { get; set; } // Navigation property
    }

}
