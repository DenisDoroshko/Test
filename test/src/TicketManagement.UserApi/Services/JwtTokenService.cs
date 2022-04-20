using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TicketManagement.UserApi.Models;
using TicketManagement.UserApi.Settings;

namespace TicketManagement.UserApi.Services
{
    /// <summary>
    /// Provides operation with jwt token.
    /// </summary>
    public class JwtTokenService
    {
        private readonly JwtTokenSettings _settings;
        private readonly UserManager<UserModel> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="userManager">UserManager.</param>
        public JwtTokenService(IOptions<JwtTokenSettings> options, UserManager<UserModel> userManager)
        {
            _settings = options.Value;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates new token.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="roles">User roles.</param>
        /// <returns>Token.</returns>
        public string GetToken(UserModel user, IList<string> roles)
        {
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            var userClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            userClaims.AddRange(roleClaims);
            var jwt = new JwtSecurityToken(
                issuer: _settings.JwtIssuer,
                audience: _settings.JwtAudience,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSecretKey)),
                    SecurityAlgorithms.HmacSha256),
                claims: userClaims);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        /// <summary>
        /// Performs token validation.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <returns>Result of validation.</returns>
        public async Task<bool> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _settings.JwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = _settings.JwtAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSecretKey)),
                    ValidateLifetime = false,
                },
                out var _);
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var currentUserId = jwtToken.Claims.First(t => t.Type == ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(currentUserId);
                var tokenRoles = jwtToken.Claims.Where(t => t.Type == ClaimTypes.Role).Select(t => t.Value);
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in tokenRoles)
                {
                    if (!roles.Contains(role))
                    {
                        throw new InvalidOperationException($"User doesn't have '{role}' role");
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
