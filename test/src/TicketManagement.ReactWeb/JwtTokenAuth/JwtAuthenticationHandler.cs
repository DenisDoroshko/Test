using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TicketManagement.ReactWeb.Clients;

namespace TicketManagement.ReactWeb.JwtTokenAuth
{
    /// <summary>
    /// Custom jwt authentication handler.
    /// </summary>
    public class JwtAuthenticationHandler : AuthenticationHandler<JwtAuthenticationOptions>
    {
        private readonly ILoginClient _loginClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="encoder">Encoder.</param>
        /// <param name="clock">Clock.</param>
        /// <param name="loginClient">Login client.</param>
        public JwtAuthenticationHandler(
            IOptionsMonitor<JwtAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ILoginClient loginClient)
            : base(options, logger, encoder, clock)
        {
            _loginClient = loginClient;
        }

        /// <summary>
        /// Performs authentication with jwt token.
        /// </summary>
        /// <returns>Result of authentication.</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            var token = Request.Headers["Authorization"].ToString()["Bearer ".Length..];
            try
            {
                await _loginClient.ValidateToken(token);
            }
            catch (HttpRequestException)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
