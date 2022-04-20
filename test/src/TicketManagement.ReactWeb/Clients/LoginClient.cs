using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TicketManagement.ReactWeb.Models;

namespace TicketManagement.ReactWeb.Clients
{
    /// <summary>
    /// Provides operations for interacting with the logins in UserApi.
    /// </summary>
    public class LoginClient : BaseClient, ILoginClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginClient"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Http accessor.</param>
        /// <param name="httpClient">Http client.</param>
        public LoginClient(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
            : base(httpContextAccessor, httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<string> Login(LoginModel loginModel, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonConvert.SerializeObject(loginModel);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await _httpClient.PostAsync("api/logins/login", httpContent, cancellationToken);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsStringAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<string> Register(RegistrationModel registrationModel, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonConvert.SerializeObject(registrationModel);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await _httpClient.PostAsync("api/logins/register", httpContent, cancellationToken);
            return await result.Content.ReadAsStringAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<string> RefreshToken(string id, CancellationToken cancellationToken = default)
        {
            var address = $"api/logins/{id}/refresh-token";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsStringAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> ValidateToken(string token, CancellationToken cancellationToken = default)
        {
            var address = $"api/logins/validate?token={token}";
            var message = await _httpClient.GetAsync(address, cancellationToken);
            return message.IsSuccessStatusCode;
        }
    }
}
