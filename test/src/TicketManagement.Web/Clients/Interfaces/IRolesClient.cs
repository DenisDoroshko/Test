using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TicketManagement.Web.Clients
{
    /// <summary>
    /// Provides operations for interacting with roles in the User api.
    /// </summary>
    public interface IRolesClient
    {
        /// <summary>
        /// Add role to user.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="role">Role.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of operation.</returns>
        Task<bool> AddToTole(string id, string role, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes user from role.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="role">Role.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of operation.</returns>
        Task<bool> DeleteFromTole(string id, string role, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets user roles.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User roles list.</returns>
        Task<List<string>> GetUserRoles(string id, CancellationToken cancellationToken = default);
    }
}
