using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TicketManagement.EventManagerApi.Client
{
    /// <summary>
    /// Provides functionality to interact with UserApi.
    /// </summary>
    public class UserClient : IUserClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserClient"/> class.
        /// </summary>
        /// <param name="httpClient">Http client.</param>
        public UserClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task ValidateToken(string token, CancellationToken cancellationToken = default)
        {
            var address = $"api/logins/validate?token={token}";
            var message = await _httpClient.GetAsync(address, cancellationToken);
            message.EnsureSuccessStatusCode();
        }
    }
}
