using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalrApi.TokenHelper;

namespace SignalrApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticateService _authService;
        public AuthController(IAuthenticateService authService)
        {
            this._authService = authService;
        }

        [HttpGet]
        public IActionResult GetToken([FromQuery] LoginRequestDTO request)
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

        [HttpPost]
        public IActionResult PostToken([FromBody] LoginRequestDTO request)
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
