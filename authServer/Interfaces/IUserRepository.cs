using System.Threading.Tasks;

namespace AuthServer.Interfaces
{
    public interface IUserRepository
    {
        Task<IUser> GetUserByUsernameAsync(string username);
        Task<IUser> GetUserByEmailAsync(string email);
        Task CreateUserAsync(IUser user);
        Task<bool> UserNameExistsAsync(string username);
        Task<bool> UserEmailExistsAsync(string email);
        Task SetLastLoginAsync(IUser user);
        Task<bool> ValidateRegistrationCodeAsync(string verifyCode);
        Task<IUser> ValidateResetCodeAsync(string resetCode);
        Task<string> ResetPasswordAsync(string userName, string email, string answer);
        Task<string> GetUserAnswerAsync(string userName);
        Task<bool> ChangePasswordAsync(string userName, string newPassword);
        void RefreshUserData();
    }
}