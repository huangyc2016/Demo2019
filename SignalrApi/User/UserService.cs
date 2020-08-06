using SignalrApi.TokenHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalrApi.User
{
    public class UserService : IUserService
    {
        public bool IsValid(LoginRequestDTO req)
        {
            return true;
        }

    }
}
