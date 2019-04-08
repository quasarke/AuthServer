using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Interfaces;

namespace AuthServer.Providers
{
    public class UserDataProvider: IUserRepository, IDisposable
    {
        public Task<IUser> GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<IUser> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task CreateUserAsync(IUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserNameExistsAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserEmailExistsAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task SetLastLoginAsync(IUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateRegistrationCodeAsync(string verifyCode)
        {
            throw new NotImplementedException();
        }

        public Task<IUser> ValidateResetCodeAsync(string resetCode)
        {
            throw new NotImplementedException();
        }

        public Task<string> ResetPasswordAsync(string userName, string email, string answer)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserAnswerAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangePasswordAsync(string userName, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void RefreshUserData()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
