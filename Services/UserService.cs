using TodoApp.DTOs.User;
using TodoApp.Models;
using TodoApp.Repositories;
using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApp.Exceptions; // Importing the CustomApiException

namespace TodoApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        // Constructor to inject the UserRepository
        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            this._userRepository = userRepository;
            this._configuration = configuration;
        }

        // Method to generate JWT token
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);  // Generate the token as a string
        }

        // Method for hashing password
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Method for verifying password
        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }

        // Register user
        public async Task<UserDto> RegisterUser(RegisterUserDto registerDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new CustomApiException("User with this email already exists.", 400); // Bad Request
            }

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password)
            };

            var createdUser = await _userRepository.CreateUserAsync(user);

            var userDto = new UserDto
            {
                UserId = createdUser.UserId,
                Username = createdUser.Username,
                Email = createdUser.Email
            };

            return userDto;
        }

        // Get user by email
        public async Task<UserDto> GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new CustomApiException("Email cannot be null or empty.", 400); // Bad Request

            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
                throw new CustomApiException("User not found.", 404); // Not Found

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email
            };
        }

        // Get user by ID
        public async Task<UserDto> GetUserById(int userId)
        {
            if (userId <= 0)
                throw new CustomApiException("User ID must be a positive integer.", 400); // Bad Request

            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
                throw new CustomApiException("User not found.", 404); // Not Found

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email
            };
        }

        // Change user password
        public async Task<Tuple<bool, string>> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(userId);
            if (existingUser == null)
                throw new Exception("User not found.");

            bool isOldPasswordValid = VerifyPassword(oldPassword, existingUser.PasswordHash);
            if (!isOldPasswordValid)
                throw new Exception("The old password is incorrect.");

            // Hash the new password
            string newHashedPassword = HashPassword(newPassword);

            // Update the user's password
            existingUser.PasswordHash = newHashedPassword;

            try
            {
                var updatedUser = await _userRepository.UpdateUserAsync(existingUser);
                if (updatedUser != null)
                {
                    // Generate a new token after password change
                    string newToken = GenerateJwtToken(updatedUser);
                    // Return a tuple with success status and the new token
                    return new Tuple<bool, string>(true, newToken);
                }
                return new Tuple<bool, string>(false, null);  // If update fails, return failure and null token
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the password.", ex);
            }
        }



        // Authenticate user
        public async Task<string> AuthenticateUser(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                throw new CustomApiException("Email and Password cannot be empty.", 400); // Bad Request
            }

            var existingUser = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (existingUser == null)
            {
                throw new CustomApiException("User not found.", 404); // Not Found
            }

            bool isPasswordValid = VerifyPassword(loginDto.Password, existingUser.PasswordHash);
            if (!isPasswordValid)
            {
                throw new CustomApiException("Invalid credentials.", 401); // Unauthorized
            }

            string token = GenerateJwtToken(existingUser);
            return token;
        }

        // Delete user account
        public async Task<bool> DeleteAccountAsync(int userId)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(userId);
            if (existingUser == null)
                throw new CustomApiException("User not found.", 404); // Not Found

            try
            {
                await _userRepository.DeleteUserAsync(existingUser);
                return true;
            }
            catch (Exception ex)
            {
                throw new CustomApiException("An error occurred while deleting the account.", 500); // Internal Server Error
            }
        }
    }
}
