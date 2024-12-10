using TodoApp.DTOs.User;
using TodoApp.Models;

namespace TodoApp.Services
{
    public interface IUserService
    {


        Task<UserDto> RegisterUser(RegisterUserDto registerDto);

        Task<UserDto> GetUserByEmail(string email);


        Task<string> AuthenticateUser(LoginDto loginDto);

        Task<UserDto> GetUserById(int userId);

        Task<Tuple<bool, string?>>  ChangePassword(int userId, string oldPassword, string newPassword);

     
        Task<bool> DeleteAccountAsync(int userId);


    }
}
