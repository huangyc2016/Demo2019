using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swagger.TokenHelper;

namespace Swagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticateService _authService;
        public AuthController(IAuthenticateService authService)
        {
            this._authService = authService;
        }
        
        [HttpPost]
        public IActionResult GetToken([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }

            object token;
            if (_authService.IsAuthenticated(request, out token))
            {
                return Ok(token);
            }
            return BadRequest("Invalid Request");

        }
    }
}
