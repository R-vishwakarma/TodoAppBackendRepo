using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TodoApp.Models;
using TodoApp.Services;
using TodoApp.DTOs.Todo;
using TodoApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using static TodoApp.DTOs.Todo.TodoUpdateDto;
using TodoApp.Enums;
using TodoApp.Exceptions;
namespace TodoApp.Controllers
{
    [Authorize]
    [Route("/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            this._todoService = todoService;
        }

        [HttpPost("addTodo")]
        public async Task<IActionResult> CreateTodo([FromBody] CreateTodoDto createTodoDto)
        {
            if (createTodoDto == null)
            {
                return BadRequest("Todo data cannot be null.");
            }

            try
            {
                var userId = ClaimHelper.GetUserIdFromClaims(HttpContext.User);
                var createdTodo = await _todoService.CreateTodoAsync(createTodoDto, userId.Value);
                return CreatedAtAction(nameof(GetTodoById), new { id = createdTodo.Id }, createdTodo);
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid Todo ID." });

                var userId = ClaimHelper.GetUserIdFromClaims(HttpContext.User);
                if (userId == null)
                    return Unauthorized(new { Message = "User is not authenticated." });

                var todo = await _todoService.GetTodoByIdAsync(id, userId.Value);

                if (todo == null)
                    return NotFound(new { Message = $"Todo with ID {id} was not found." });

                return Ok(todo);
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoDto updateTodoDto)
        {
            try
            {
                var userId = ClaimHelper.GetUserIdFromClaims(HttpContext.User);
                var updatedTodo = await _todoService.UpdateTodoAsync(id, userId.Value, updateTodoDto);

                if (updatedTodo == null)
                    return NotFound(new { Message = "Todo not found or access denied." });

                return Ok(updatedTodo);
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            try
            {
                var userId = ClaimHelper.GetUserIdFromClaims(HttpContext.User);
                var isDeleted = await _todoService.DeleteTodoAsync(id, userId.Value);

                if (!isDeleted)
                    return NotFound(new { Message = "Todo item not found or access denied." });

                return Ok(new { Message = "Todo item deleted successfully." });
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }

        [HttpGet("getAllTodos")]
        public async Task<IActionResult> GetAllTodos()
        {
            try
            {
                var userId = ClaimHelper.GetUserIdFromClaims(HttpContext.User);
                var todos = await _todoService.GetAllTodosAsync(userId.Value);

                if (todos == null || !todos.Any())
                    return NotFound(new { Message = "No todos found." });

                return Ok(todos);
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("byPriority")]
        public async Task<IActionResult> GetTodosByPriority([FromQuery] PriorityLevel priority)
        {
            try
            {
                var userId = ClaimHelper.GetUserIdFromClaims(HttpContext.User);
                var todos = await _todoService.GetTodosByPriorityAsync(priority, userId.Value);

                if (!todos.Any())
                    return NotFound("No Todos found with the specified priority.");

                return Ok(todos);
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }

        [HttpGet("byCategory")]
        public async Task<IActionResult> GetTodosByCategory([FromQuery] TodoCategory category)
        {
            try
            {
                var userId = ClaimHelper.GetUserIdFromClaims(HttpContext.User);
                var todos = await _todoService.GetTodosByCategoryAsync(category, userId.Value);

                if (!todos.Any())
                    return NotFound("No Todos found with the specified category.");

                return Ok(todos);
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }

        [HttpPatch("toggleComplete/{id:int}")]
        public async Task<IActionResult> ToggleTodoCompletion(int id)
        {
            try
            {
                var userId = ClaimHelper.GetUserIdFromClaims(HttpContext.User);
                var updatedTodo = await _todoService.ToggleTodoCompletionAsync(id, userId.Value);
                return Ok(updatedTodo);
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }

        [HttpGet("query")]
        public async Task<IActionResult> SearchTodos([FromQuery] string? keyword)
        {
            try
            {
                var userId = ClaimHelper.GetUserIdFromClaims(HttpContext.User);
                var todos = await _todoService.SearchTodosAsync(userId.Value, keyword);
                return Ok(todos);
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }
    }

}
