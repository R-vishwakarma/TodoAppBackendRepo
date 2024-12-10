using TodoApp.DTOs.Todo;
using TodoApp.Models;
using TodoApp.Repositories;
using TodoApp.Enums;
using static TodoApp.DTOs.Todo.TodoUpdateDto;
using Microsoft.EntityFrameworkCore;
using TodoApp.Exceptions; // Import CustomApiException

namespace TodoApp.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;

        public TodoService(ITodoRepository todoRepository)
        {
            this._todoRepository = todoRepository;
        }

        // Create Todo
        public async Task<Todo> CreateTodoAsync(CreateTodoDto createTodoDto, int userId)
        {
            if (createTodoDto == null)
                throw new CustomApiException("Todo details cannot be null.", 400); // Bad Request

            if (userId <= 0)
                throw new CustomApiException("Invalid user ID.", 400); // Bad Request

            var todo = new Todo
            {
                Title = createTodoDto.Title,
                Description = createTodoDto.Description,
                Priority = Enum.Parse<PriorityLevel>(createTodoDto.Priority, true),
                Category = Enum.Parse<TodoCategory>(createTodoDto.Category, true),
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            try
            {
                var createdTodo = await _todoRepository.CreateTodoAsync(todo);
                return createdTodo;
            }
            catch (Exception ex)
            {
                throw new CustomApiException("An error occurred while creating the Todo.", 500, ex); // Internal Server Error
            }
        }

        // Get Todo by ID
        public async Task<TodoResponseDto> GetTodoByIdAsync(int todoId, int userId)
        {
            if (todoId <= 0)
                throw new CustomApiException("Todo ID must be a positive number.", 400); // Bad Request

            var todo = await _todoRepository.GetTodoByIdAndUserIdAsync(todoId, userId);

            if (todo == null)
                throw new CustomApiException("Todo not found or you do not have access.", 404); // Not Found


          
            return new TodoResponseDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                Priority =todo.Priority,
                Category =todo.Category,

                 IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            };
        }

        // Update Todo
        public async Task<Todo?> UpdateTodoAsync(int todoId, int userId, UpdateTodoDto updateTodoDto)
        {
            var todo = await _todoRepository.GetTodoByIdAndUserIdAsync(todoId, userId);

            if (todo == null)
                throw new CustomApiException("Todo not found or access denied.", 403); // Forbidden

            if (!string.IsNullOrWhiteSpace(updateTodoDto.Title))
                todo.Title = updateTodoDto.Title;

            if (!string.IsNullOrWhiteSpace(updateTodoDto.Description))
                todo.Description = updateTodoDto.Description;

            if (!string.IsNullOrWhiteSpace(updateTodoDto.Priority))
                todo.Priority = Enum.Parse<PriorityLevel>(updateTodoDto.Priority, true);

            if (!string.IsNullOrWhiteSpace(updateTodoDto.Category))
                todo.Category = Enum.Parse<TodoCategory>(updateTodoDto.Category, true);

            if (updateTodoDto.IsCompleted.HasValue)
                todo.IsCompleted = updateTodoDto.IsCompleted.Value;

            var updatedTodo = await _todoRepository.UpdateTodoAsync(todo);
            return updatedTodo;
        }

        // Delete Todo
        public async Task<bool> DeleteTodoAsync(int todoId, int userId)
        {
            var todo = await _todoRepository.GetTodoByIdAndUserIdAsync(todoId, userId);

            if (todo == null)
                return false;

            await _todoRepository.DeleteTodoAsync(todo);
            return true;
        }

        // Get all Todos
        public async Task<IEnumerable<Todo>> GetAllTodosAsync(int userId)
        {
            return await _todoRepository.GetAllTodosByUserIdAsync(userId);
        }

        // Get Todos by Priority
        public async Task<IEnumerable<Todo>> GetTodosByPriorityAsync(PriorityLevel priority, int userId)
        {
            return await _todoRepository.GetTodosByPriorityAsync(priority, userId);
        }

        // Get Todos by Category
        public async Task<IEnumerable<Todo>> GetTodosByCategoryAsync(TodoCategory category, int userId)
        {
            return await _todoRepository.GetTodosByCategoryAsync(category, userId);
        }

        // Toggle Todo Completion
        public async Task<Todo> ToggleTodoCompletionAsync(int todoId, int userId)
        {
            var todo = await _todoRepository.GetTodoByIdAndUserIdAsync(todoId, userId);

            if (todo == null)
                throw new CustomApiException($"Todo with ID {todoId} not found.", 404); // Not Found

            todo.IsCompleted = !todo.IsCompleted;
            return await _todoRepository.UpdateTodoAsync(todo);
        }

        // Search Todos
        public async Task<IEnumerable<Todo>> SearchTodosAsync(int userId, string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await _todoRepository.GetAllTodosByUserIdAsync(userId);
            }

            return await _todoRepository.SearchTodosAsync(userId, keyword);
        }
    }
}
