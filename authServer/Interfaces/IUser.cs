using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthServer.Interfaces
{
    public interface IUser
    {
        string UserName { get; set; }
        string Password { get; set; }
        string Salt { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string EmailAddress { get; set; }
        string PhoneNumber { get; set; }
        bool IsEmailConfirmed { get; set; }
        string EmailConfirmationCode { get; set; }
        string PasswordResetCode { get; set; }
        DateTimeOffset? LastLoginTime { get; set; }
        bool IsActive { get; set; }
        bool? IsDeleted { get; set; }
        int? DeleterUserId { get; set; }
        DateTimeOffset? DeletionTime { get; set; }
        DateTimeOffset? LastModificationTime { get; set; }
        int? LastModifierUserId { get; set; }
        DateTimeOffset? CreationTime { get; set; }
        int? CreatorUserId { get; set; }
        string SecurityQuestion { get; set; }
        string SecurityAnswer { get; set; }
    }
}