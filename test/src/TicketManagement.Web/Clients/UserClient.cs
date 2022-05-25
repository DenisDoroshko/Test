using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TicketManagement.Web.Models;

namespace TicketManagement.Web.Clients
{
    /// <summary>
    /// Provides operations for interacting with the User api.
    /// </summary>
    public class UserClient : BaseClient, IUserClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserClient"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Http accessor.</param>
        /// <param name="httpClient">Http client.</param>
        public UserClient(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
            : base(httpContextAccessor, httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<UserViewModel> Get(string id, CancellationToken cancellationToken = default)
        {
            var address = $"api/users/{id}";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            result.EnsureSuccessStatusCode();
            var jsonUser = await result.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<UserViewModel>(jsonUser);
        }

        /// <inheritdoc />
        public async Task<List<UserViewModel>> Get(CancellationToken cancellationToken = default)
        {
            var address = $"api/users";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            result.EnsureSuccessStatusCode();
            var jsonUsers = await result.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<List<UserViewModel>>(jsonUsers);
        }

        /// <inheritdoc />
        public async Task<UserViewModel> GetByName(string name, CancellationToken cancellationToken = default)
        {
            var address = $"api/users/name/{name}";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            result.EnsureSuccessStatusCode();
            var jsonUser = await result.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<UserViewModel>(jsonUser);
        }

        /// <inheritdoc />
        public async Task<UserViewModel> GetByEmail(string email, CancellationToken cancellationToken = default)
        {
            var address = $"api/users/email/{email}";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            result.EnsureSuccessStatusCode();
            var jsonUser = await result.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<UserViewModel>(jsonUser);
        }

        /// <inheritdoc />
        public async Task<bool> Put(UserViewModel user, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonConvert.SerializeObject(user);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await _httpClient.PutAsync($"api/users", httpContent, cancellationToken);
            return result.IsSuccessStatusCode;
        }

        /// <inheritdoc />
        public async Task<bool> ChangePassword(string id, string oldPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonConvert.SerializeObject(new { oldPassword, newPassword });
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await _httpClient.PutAsync($"api/users/{id}/password", httpContent, cancellationToken);
            return result.IsSuccessStatusCode;
        }

        /// <inheritdoc />
        public async Task<bool> Delete(string id, CancellationToken cancellationToken = default)
        {
            var address = $"api/users/{id}";
            var result = await _httpClient.DeleteAsync(address, cancellationToken);
            return result.IsSuccessStatusCode;
        }
    }
}
