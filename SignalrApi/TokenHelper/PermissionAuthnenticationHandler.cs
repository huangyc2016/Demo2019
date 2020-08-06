using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalrApi.TokenHelper
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string DeniedAction { get; set; }
        public string LoginPath { get; set; }
        public string ClaimType { get; set; }
        public TimeSpan Expiration { get; set; }
        public PermissionRequirement(string deniedAction, string loginPath, string claimType, TimeSpan expiration)
        {
            DeniedAction = deniedAction;
            LoginPath = loginPath;
            ClaimType = claimType;
            Expiration = expiration;
        }
    }
    public class PermissionAuthnenticationHandler : AuthorizationHandler<PermissionRequirement>
    {
        //public IAuthenticationSchemeProvider Scheme;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAuthnenticationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            throw new NotImplementedException();
        }
    }
}
