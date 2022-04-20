using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TicketManagement.EventManagerApi.Client
{
    /// <summary>
    /// Represents client for interacting with UserApi.
    /// </summary>
    public interface IUserClient
    {
        /// <summary>
        /// Sends request to validate token.
        /// </summary>
        /// <param name="token">Authentication token.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        Task ValidateToken(string token, CancellationToken cancellationToken = default);
    }
}
