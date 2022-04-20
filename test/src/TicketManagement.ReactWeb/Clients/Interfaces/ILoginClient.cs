using System.Threading;
using System.Threading.Tasks;
using TicketManagement.ReactWeb.Models;

namespace TicketManagement.ReactWeb.Clients
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
        Task<string> Login(LoginModel loginModel, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers user.
        /// </summary>
        /// <param name="registrationModel">Registration model.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Token.</returns>
        Task<string> Register(RegistrationModel registrationModel, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates token.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Is token valid.</returns>
        Task<bool> ValidateToken(string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes token.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>New token.</returns>
        Task<string> RefreshToken(string id, CancellationToken cancellationToken = default);
    }
}
