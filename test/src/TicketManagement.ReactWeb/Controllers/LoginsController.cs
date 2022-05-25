using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TicketManagement.ReactWeb.Clients;
using TicketManagement.ReactWeb.Models;

namespace TicketManagement.ReactWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginsController : Controller
    {
        private readonly IStringLocalizer<LoginsController> _localizer;
        private readonly ILoginClient _loginClient;
        private readonly IUserClient _userClient;

        public LoginsController(IStringLocalizer<LoginsController> localizer, ILoginClient loginClient, IUserClient userClient)
        {
            _localizer = localizer;
            _loginClient = loginClient;
            _userClient = userClient;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _loginClient.Login(model);

            return Ok(result);
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshToken(string id)
        {
            var result = await _loginClient.RefreshToken(id);

            return Ok(result);
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] RegistrationModel model)
        {
            var response = new Response<string>();
            if (await _userClient.GetByName(model.Username) != null)
            {
                response.Errors.Add(_localizer["Username already exists"]);
            }

            if (await _userClient.GetByEmail(model.Email) != null)
            {
                response.Errors.Add(_localizer["Email already exists"]);
            }

            if (!response.Errors.Any())
            {
                response.Result = await _loginClient.Register(model);
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpGet("validate")]
        public async Task<IActionResult> Validate(string token)
        {
            var result = await _loginClient.ValidateToken(token);
            return result ? Ok() : Forbid();
        }
    }
}
