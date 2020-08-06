using Swagger.TokenHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swagger.User
{
    public interface IUserService
    {
        bool IsValid(LoginRequestDTO req);
    }
}
