using System.Threading;
using System.Threading.Tasks;
using TicketManagement.Web.Models;

namespace TicketManagement.Web.Clients
{
    /// <summary>
    /// Provides operations for interacting with the logins in UserApi.
    /// </summary>
    public interface ILoginClient
    {
        /// <summary>
        /// Logins user.
        /// </summary>
        /// <param name="loginModel">Login model.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Token.</returns>
        Task<string> Login(LoginViewModel loginModel, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers user.
        /// </summary>
        /// <param name="registrationModel">Registration model.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Token.</returns>
        Task<string> Register(RegistrationViewModel registrationModel, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates token.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        Task ValidateToken(string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes token.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>New token.</returns>
        Task<string> RefreshToken(string id, CancellationToken cancellationToken = default);
    }
}
