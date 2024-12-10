using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Enums;
using TodoApp.Models;
using TodoApp.Exceptions;  // Ensure to import the CustomApiException class

namespace TodoApp.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly AppDbContext _context;

        public TodoRepository(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<Todo> CreateTodoAsync(Todo todo)
        {
            if (todo == null)
                throw new CustomApiException("Todo cannot be null.", 400); // Bad Request

            try
            {
                await _context.Todos.AddAsync(todo);
                await _context.SaveChangesAsync();
                return todo;
            }
            catch (DbUpdateException dbEx)
            {
                throw new CustomApiException("Database error while adding Todo.", 500, dbEx); // Internal Server Error
            }
            catch (Exception ex)
            {
                throw new CustomApiException("Unexpected error while creating Todo.", 500, ex); // Internal Server Error
            }
        }

        public async Task<Todo?> GetTodoByIdAndUserIdAsync(int todoId, int userId)
        {
            try
            {
                var todo = await _context.Todos
                    .FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);

                if (todo == null)
                    throw new CustomApiException($"Todo with ID {todoId} not found.", 404); // Not Found

                return todo;
            }
            catch (Exception ex)
            {
                throw new CustomApiException("Error fetching Todo.", 500, ex); // Internal Server Error
            }
        }

        public async Task<Todo?> UpdateTodoAsync(Todo todo)
        {
            try
            {
                _context.Todos.Update(todo);
                await _context.SaveChangesAsync();
                return todo;
            }
            catch (DbUpdateException dbEx)
            {
                throw new CustomApiException("Database error while updating Todo.", 500, dbEx); // Internal Server Error
            }
            catch (Exception ex)
            {
                throw new CustomApiException("Unexpected error while updating Todo.", 500, ex); // Internal Server Error
            }
        }

        public async Task DeleteTodoAsync(Todo todo)
        {
            try
            {
                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new CustomApiException("Database error while deleting Todo.", 500, dbEx); // Internal Server Error
            }
            catch (Exception ex)
            {
                throw new CustomApiException("Unexpected error while deleting Todo.", 500, ex); // Internal Server Error
            }
        }

        public async Task<IEnumerable<Todo>> GetAllTodosByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Todos.Where(t => t.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new CustomApiException("Error fetching all Todos for the user.", 500, ex); // Internal Server Error
            }
        }

        public async Task<IEnumerable<Todo>> GetTodosByPriorityAsync(PriorityLevel priority, int userId)
        {
            try
            {
                return await _context.Todos
                    .Where(t => t.Priority == priority && t.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new CustomApiException("Error fetching Todos by priority.", 500, ex); // Internal Server Error
            }
        }

        public async Task<IEnumerable<Todo>> GetTodosByCategoryAsync(TodoCategory category, int userId)
        {
            try
            {
                return await _context.Todos
                    .Where(t => t.Category == category && t.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new CustomApiException("Error fetching Todos by category.", 500, ex); // Internal Server Error
            }
        }

        public async Task<IEnumerable<Todo>> SearchTodosAsync(int userId, string? keyword)
        {
            try
            {
                var query = _context.Todos.Where(t => t.UserId == userId);

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(t => EF.Functions.Like(t.Title, $"%{keyword}%") ||
                                             EF.Functions.Like(t.Description, $"%{keyword}%"));
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new CustomApiException("Error searching Todos.", 500, ex); // Internal Server Error
            }
        }
    }
}
