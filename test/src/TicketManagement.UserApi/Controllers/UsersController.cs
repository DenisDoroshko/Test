using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.UserApi.Models;

namespace TicketManagement.UserApi.Controllers
{
    /// <summary>
    /// Represents api for user management.
    /// </summary>
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<UserModel> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        public UsersController(UserManager<UserModel> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>Users list.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        [HttpGet]
        public IEnumerable<UserModel> Get()
        {
            return _userManager.Users.ToList();
        }

        /// <summary>
        /// Gets user by id.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>User.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return Ok(user);
        }

        /// <summary>
        /// Gets user by name.
        /// </summary>
        /// <param name="name">User name.</param>
        /// <returns>User.</returns>
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            return Ok(user);
        }

        /// <summary>
        /// Gets user by email.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <returns>User.</returns>
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return Ok(user);
        }

        /// <summary>
        /// Updates user information.
        /// </summary>
        /// <param name="user">User to update.</param>
        /// <returns>Result of operation.</returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserModel user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var userToUpdate = await _userManager.FindByNameAsync(user.UserName);
            userToUpdate.Name = user.Name;
            userToUpdate.Surname = user.Surname;
            userToUpdate.TimeZoneId = user.TimeZoneId;
            userToUpdate.Balance = user.Balance;
            userToUpdate.Email = user.Email;
            var result = await _userManager.UpdateAsync(userToUpdate);
            return result.Succeeded ? Ok() : BadRequest();
        }

        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>Result of operation.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded ? Ok() : BadRequest();
        }

        /// <summary>
        /// Changes user password.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="changePasswordModel">Change password model.</param>
        /// <returns>Result of operation.</returns>
        [Authorize]
        [HttpPut("{id}/password")]
        public async Task<IActionResult> ChangePassword([FromRoute] string id, [FromBody]ChangePasswordModel changePasswordModel)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.ChangePasswordAsync(user, changePasswordModel.OldPassword, changePasswordModel.NewPassword);
            return result.Succeeded ? Ok() : BadRequest();
        }
    }
}
