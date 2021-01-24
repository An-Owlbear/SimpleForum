using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.CrossConnection
{
    // Authenticates a request from another instance

    public class AuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string Scheme = "CrossServerAuthentication";
    }
    
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationOptions>
    {
        private readonly SimpleForumRepository _repository;

        public AuthenticationHandler(
            IOptionsMonitor<AuthenticationOptions> options, 
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            SimpleForumRepository repository)
            : base (options, logger, encoder, clock)
        {
            _repository = repository;
        }
        
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization")) return AuthenticateResult.Fail("Unauthorized");

            string authHeader = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader)) return AuthenticateResult.Fail("Unauthorized");
            if (!authHeader.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase)) return 
                AuthenticateResult.Fail("Unauthorized");

            string token = authHeader.Substring("bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token)) return AuthenticateResult.Fail("Unauthorized");

            return await verifyToken(token);
        }

        // Returns true if the token is valid, else false
        private async Task<AuthenticateResult> verifyToken(string token)
        {
            IncomingServerToken serverToken = await _repository.GetIncomingServerTokenAsync(token);
            if (serverToken == null) return AuthenticateResult.Fail("Unauthorized");

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, serverToken.Address),
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}