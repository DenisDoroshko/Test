using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketManagement.ReactWeb.Models;

namespace TicketManagement.ReactWeb.Clients
{
    /// <summary>
    /// Provides operations for interacting with the User api.
    /// </summary>
    public interface IUserClient
    {
        /// <summary>
        /// Gets user by id.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User.</returns>
        Task<UserModel> Get(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets users.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Users list.</returns>
        Task<List<UserModel>> Get(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets user by name.
        /// </summary>
        /// <param name="name">Username.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User.</returns>
        Task<UserModel> GetByName(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets user by email.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User.</returns>
        Task<UserModel> GetByEmail(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User.</returns>
        Task<bool> Delete(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of operation.</returns>
        Task<bool> Put(UserModel user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes password.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="oldPassword">Old password.</param>
        /// <param name="newPassword">New password.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of operation.</returns>
        Task<bool> ChangePassword(string id, string oldPassword, string newPassword, CancellationToken cancellationToken = default);
    }
}
