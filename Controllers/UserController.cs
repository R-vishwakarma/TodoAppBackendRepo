using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Services;
using TodoApp.DTOs.User;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TodoApp.Helpers;
using TodoApp.Exceptions; // Importing CustomApiException

namespace TodoApp.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // User login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _userService.AuthenticateUser(loginDto);
                return Ok(new
                {
                    Token = token,
                    Message = "Login successful.."
                });
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during login.", Details = ex.Message });
            }
        }

        // Register user
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerDto)
        {
            if (registerDto == null)
                return BadRequest("Invalid user data.");

            try
            {
                var user = await _userService.RegisterUser(registerDto);
                var response = new
                {
                    Message = "User registered successfully!",
                    User = user
                };

                return CreatedAtAction(nameof(GetUserById), new { userId = user.UserId }, response);
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the user.", Details = ex.Message });
            }
        }

        // Get user by ID
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _userService.GetUserById(userId);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the user.", Details = ex.Message });
            }
        }

        // Change user password
        //user password change

        [Authorize]
        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var userId = ClaimHelper.GetUserIdFromClaims(HttpContext.User);

            if (userId == null)
            {
                return Unauthorized("Invalid token.");
            }

            try
            {
                var (isPasswordChanged, newToken) = await _userService.ChangePassword(userId.Value, dto.OldPassword, dto.NewPassword);

                if (!isPasswordChanged)
                {
                    return BadRequest("Password change failed.");
                }

                return Ok(new { Message = "Password changed successfully.", Token = newToken });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        // Delete user account
        [Authorize]
        [HttpDelete("deleteAccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            var claimsPrincipal = HttpContext.User;

            // Extract UserId from claims
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid token.");
            }

            try
            {
                var isDeleted = await _userService.DeleteAccountAsync(userId);

                if (!isDeleted)
                    return BadRequest("Account deletion failed.");

                return Ok(new { Message = "Account deleted successfully." });
            }
            catch (CustomApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the account.", Details = ex.Message });
            }
        }
    }
}
