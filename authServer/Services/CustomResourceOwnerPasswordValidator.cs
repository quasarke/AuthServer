using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace AuthServer.Services
{
    public class ResourceOwnerPasswordValidator: IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
