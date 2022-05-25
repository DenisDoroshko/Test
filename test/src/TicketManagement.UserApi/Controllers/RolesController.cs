using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.UserApi.Models;

namespace TicketManagement.UserApi.Controllers
{
    /// <summary>
    /// Represents api for roles management.
    /// </summary>
    [Route("api/[controller]")]
    public class RolesController : Controller
    {
        private readonly UserManager<UserModel> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesController"/> class.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        public RolesController(UserManager<UserModel> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Gets user roles.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>List of user roles.</returns>
        [HttpGet("{id}")]
        public async Task<IEnumerable<string>> GetUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        /// <summary>
        /// Add role to user.
        /// </summary>
        /// <param name="role">Tole.</param>
        /// <param name="id">User id.</param>
        /// <returns>Result of operation.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        [HttpPost("{role}/{id}")]
        public async Task<IActionResult> AddToRole(string role, string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        /// <summary>
        /// Deletes role from user.
        /// </summary>
        /// <param name="role">Role.</param>
        /// <param name="id">User id.</param>
        /// <returns>Result of operation.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        [HttpDelete("{role}/{id}")]
        public async Task<IActionResult> DeleteFromRole(string role, string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.RemoveFromRoleAsync(user, role);
            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }
    }
}
