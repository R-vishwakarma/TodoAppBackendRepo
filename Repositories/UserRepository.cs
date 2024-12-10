using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Exceptions; // Importing CustomApiException

namespace TodoApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (user == null)
            {
                throw new CustomApiException("User cannot be null.", 400); // Bad Request
            }

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException ex)
            {
                throw new CustomApiException("An error occurred while Adding the user .", 500, ex); // Internal Server Error
            }
            catch (Exception ex)
            {
                throw new CustomApiException("An unexpected error occurred.", 500, ex); // Internal Server Error
            }
        }

        // Delete user
        public async Task<bool> DeleteUserAsync(User user)
        {
            if (user == null)
            {
                throw new CustomApiException("User cannot be null.", 400); // Bad Request
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new CustomApiException("An error occurred while deleting the user .", 500, ex); // Internal Server Error
            }
        }

        // Get user by emailId
        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new CustomApiException("Email cannot be null or empty.", 400); // Bad Request

            try
            {
                return await _context.Users.SingleOrDefaultAsync(e => e.Email == email);
            }
            catch (Exception ex)
            {
                throw new CustomApiException("An error occurred while retrieving the user by email.", 500, ex); // Internal Server Error
            }
        }

        // Get user by ID
        public async Task<User> GetUserByIdAsync(int userId)
        {
            if (userId <= 0)
                throw new CustomApiException("User ID must be a positive number.", 400); // Bad Request

            try
            {
                return await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
            }
            catch (Exception ex)
            {
                throw new CustomApiException("An error occurred while retrieving the user by ID.", 500, ex); // Internal Server Error
            }
        }

        // Update user
        public async Task<User> UpdateUserAsync(User user)
        {
            if (user == null)
                throw new CustomApiException("Invalid user details.", 400); // Bad Request

            try
            {
                var existingUser = await _context.Users.FindAsync(user.UserId);
                if (existingUser == null)
                    throw new CustomApiException("User not found.", 404); // Not Found

                // Update the user
                _context.Entry(existingUser).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
                return existingUser;
            }
            catch (Exception ex)
            {
                throw new CustomApiException("An error occurred while updating the user.", 500, ex); // Internal Server Error
            }
        }
    }
}
