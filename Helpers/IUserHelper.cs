using Microsoft.AspNetCore.Identity;
using TSShopping.Data.Entities;
using TSShopping.Models;

namespace TSShopping.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserAsync(string email);
        Task<IdentityResult> AddUserAsync(User user, string password);
        Task CheckRoleAsync(string roleName);
        Task AddUserToRoleAsync(User user, string roleName);
        Task<bool> IsUserInRoleAsync(User user, string roleName);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<User> AddUserAsync(AddUserViewModel model);
        
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);
        Task<IdentityResult> UpdateUserAsync(User user);             
        Task<User> GetUserAsync(Guid userId);

    }
}