using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TicketManagement.ReactWeb.Clients;
using TicketManagement.ReactWeb.Models;

namespace TicketManagement.ReactWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IStringLocalizer<LoginsController> _localizer;
        private readonly IUserClient _userClient;
        private readonly ILoginClient _loginClient;
        private readonly IRolesClient _rolesClient;

        public UsersController(IStringLocalizer<LoginsController> localizer, IUserClient userClient, ILoginClient loginClient, IRolesClient rolesClient)
        {
            _localizer = localizer;
            _userClient = userClient;
            _loginClient = loginClient;
            _rolesClient = rolesClient;
        }

        [HttpPost]
        [Authorize(Roles.VenueManager)]
        public async Task<IActionResult> Create([FromBody] RegistrationModel model)
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

        [HttpPost("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (model is not { Id: not null, OldPassword: not null, NewPassword: not null })
            {
                return BadRequest();
            }

            var result = await _userClient.ChangePassword(model.Id, model.OldPassword, model.NewPassword);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<UserModel> GetUser(string id)
        {
            return await _userClient.Get(id);
        }

        [HttpGet]
        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            var users = await _userClient.Get();
            foreach (var user in users)
            {
                user.Roles = (await _rolesClient.GetUserRoles(user.Id)).ToList();
            }

            return users;
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> EditAccount([FromBody] UserModel model)
        {
            var user = await _userClient.Get(model.Id);
            if (user != null)
            {
                user.Name = model.Name;
                user.Surname = model.Surname;
                user.Email = model.Email;
                user.TimeZoneId = model.TimeZoneId;
                var result = await _userClient.Put(user);
                if (result)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles=Roles.VenueManager)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userClient.Delete(id);
            return result ? Ok() : BadRequest();
        }
    }
}
