using SignalrApi.TokenHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalrApi.User
{
    public interface IUserService
    {
        bool IsValid(LoginRequestDTO req);
    }
}
