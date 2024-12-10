using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.DTOs.Todo;
using TodoApp.Enums;
using TodoApp.Models;
using static TodoApp.DTOs.Todo.TodoUpdateDto;

namespace TodoApp.Services
{
    public interface ITodoService
    {
        // Method to create a new Todo
        Task<Todo> CreateTodoAsync(CreateTodoDto createTodoDto, int userId);

        Task<Todo?> UpdateTodoAsync(int todoId, int userId, UpdateTodoDto updateTodoDto);

        Task<bool> DeleteTodoAsync(int todoId, int userId);


        Task<TodoResponseDto> GetTodoByIdAsync(int todoId, int userId);

        Task<IEnumerable<Todo>> GetAllTodosAsync(int userId);

        Task<IEnumerable<Todo>> GetTodosByPriorityAsync(PriorityLevel priority, int userId);
        Task<IEnumerable<Todo>> GetTodosByCategoryAsync(TodoCategory category, int userId);

        Task<Todo> ToggleTodoCompletionAsync(int todoId, int userId);

        Task<IEnumerable<Todo>> SearchTodosAsync(int userId, string? keyword);
    }
}
