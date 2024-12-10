
using TodoApp.Enums;
namespace TodoApp.DTOs.Todo
{
    public class CreateTodoDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string  Priority { get; set; } // Enum for Priority
        public string Category { get; set; } // Enum for Category
    }

}
