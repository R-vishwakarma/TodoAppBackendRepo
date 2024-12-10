using TodoApp.Models;

namespace TodoApp.Repositories
{
    public interface IUserRepository
    {

        Task<User> CreateUserAsync(User user);
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByEmailAsync(string email);

        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(User user);
      
    }
}
