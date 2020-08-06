using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swagger.TokenHelper
{
   public interface IAuthenticateService
    {
        bool IsAuthenticated(LoginRequestDTO request, out object token);

    }
}
