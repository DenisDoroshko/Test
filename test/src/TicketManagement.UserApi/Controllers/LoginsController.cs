using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using TicketManagement.UserApi.Models;
using TicketManagement.UserApi.Services;

namespace TicketManagement.UserApi.Controllers
{
    /// <summary>
    /// Represents api for logins management.
    /// </summary>
    [Route("api/[controller]")]
    public class LoginsController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly JwtTokenService _jwtTokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginsController"/> class.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        /// <param name="signInManager">Sign in manager.</param>
        /// <param name="jwtTokenService">Jwt token service.</param>
        public LoginsController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, JwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Logins user.
        /// </summary>
        /// <param name="model">Login model.</param>
        /// <returns>Result of operation.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (!result.Succeeded)
            {
                return Forbid();
            }

            var user = await _userManager.FindByNameAsync(model.Username);
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(_jwtTokenService.GetToken(user, roles));
        }

        /// <summary>
        /// Registers user.
        /// </summary>
        /// <param name="model">Registration model.</param>
        /// <returns>Result of operation.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegistrationModel model)
        {
            var user = new UserModel
            {
                UserName = model.Username,
                Surname = model.Surname,
                Name = model.Name,
                Email = model.Email,
                TimeZoneId = DateTimeZoneProviders.Tzdb.GetSystemDefault().Id,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, model.Role);
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(_jwtTokenService.GetToken(user, roles));
        }

        /// <summary>
        /// Refreshes token for specified user.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>Token.</returns>
        [HttpGet("{id}/refresh-token")]
        public async Task<IActionResult> RefreshToken(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest();
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(_jwtTokenService.GetToken(user, roles));
        }

        /// <summary>
        /// Validates token.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <returns>Result of validation.</returns>
        [HttpGet("validate")]
        public async Task<IActionResult> Validate(string token)
        {
            return await _jwtTokenService.ValidateToken(token) ? Ok() : Forbid();
        }
    }
}
