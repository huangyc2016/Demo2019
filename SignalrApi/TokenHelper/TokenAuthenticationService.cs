using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SignalrApi.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SignalrApi.TokenHelper
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly IUserService _userService;
        private readonly TokenManagement _tokenManagement;
        public TokenAuthenticationService(IUserService userService, IOptions<TokenManagement> tokenManagement)
        {
            _userService = userService;
            _tokenManagement = tokenManagement.Value;
        }

        public bool IsAuthenticated(LoginRequestDTO request, out object token)
        {
            token = string.Empty;
            if (!_userService.IsValid(request))
                return false;
            var cl = new[]
            {
                //new Claim(ClaimTypes.Name,request.Username),
                new Claim("UserName",request.Username),
                new Claim("Password",request.Password)
            };
            string iss = _tokenManagement.Issuer;
            string aud = _tokenManagement.Audience;
            var notb = DateTime.UtcNow;
            var exp = DateTime.UtcNow.AddSeconds(_tokenManagement.AccessTokenExpiresSeconds);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var signcreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(issuer: iss, audience: aud, claims: cl,notBefore: notb, expires: exp, signingCredentials: signcreds);
            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            token = (new
            {
                access_token = token,
                token_type = "Bearer",
            });
            return true;
        }

        /// <summary>
        /// 解析JWT字符串
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public LoginRequestDTO SerializeJWT(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            object username = new object();
            object password = new object();
            try
            {
                jwtToken.Payload.TryGetValue("UserName", out username);
                jwtToken.Payload.TryGetValue("Password", out password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var tm = new LoginRequestDTO
            {
                Username = username.ToString(),
                Password= password.ToString()
            };
            return tm;
        }
    }
}
