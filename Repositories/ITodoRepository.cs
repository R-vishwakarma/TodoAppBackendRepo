using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Enums;
using TodoApp.Models;

namespace TodoApp.Repositories
{
    public interface ITodoRepository
    {
        // Method to create a new Todo
        Task<Todo> CreateTodoAsync(Todo todo);
        Task<Todo?> UpdateTodoAsync(Todo todo);

        Task DeleteTodoAsync(Todo todo);

        // // Method to get a Todo by user id
        Task<Todo> GetTodoByIdAndUserIdAsync(int todoId, int userId);

        //get all todo of a user 
        Task<IEnumerable<Todo>> GetAllTodosByUserIdAsync(int userId);

        //get todo by pririty
        Task<IEnumerable<Todo>> GetTodosByPriorityAsync(PriorityLevel priority, int userId);

        Task<IEnumerable<Todo>> GetTodosByCategoryAsync(TodoCategory category, int userId);
        Task<IEnumerable<Todo>> SearchTodosAsync(int userId, string keyword);
    }
}
