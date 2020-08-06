using Swagger.TokenHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swagger.User
{
    public class UserService : IUserService
    {
        public bool IsValid(LoginRequestDTO req)
        {
            return true;
        }

    }
}
