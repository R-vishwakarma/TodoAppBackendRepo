using TodoApp.Enums;

namespace TodoApp.DTOs.Todo
{
    public class TodoUpdateDto
    {
        public class UpdateTodoDto
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Priority { get; set; }
            public string? Category { get; set; } 
            public bool? IsCompleted { get; set; } 
        }

    }

}
