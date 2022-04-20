using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TicketManagement.ReactWeb.Clients
{
    /// <summary>
    /// Provides operations for interacting with roles in the User api.
    /// </summary>
    public class RolesClient : BaseClient, IRolesClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesClient"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Http accessor.</param>
        /// <param name="httpClient">Http client.</param>
        public RolesClient(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
            : base(httpContextAccessor, httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<bool> AddToTole(string id, string role, CancellationToken cancellationToken = default)
        {
            var httpContent = new StringContent("", Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await _httpClient.PostAsync($"api/roles/{role}/{id}", httpContent, cancellationToken);
            return result.IsSuccessStatusCode;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteFromTole(string id, string role, CancellationToken cancellationToken = default)
        {
            var result = await _httpClient.DeleteAsync($"api/roles/{role}/{id}", cancellationToken);
            return result.IsSuccessStatusCode;
        }

        /// <inheritdoc />
        public async Task<List<string>> GetUserRoles(string id, CancellationToken cancellationToken = default)
        {
            var address = $"api/roles/{id}";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            result.EnsureSuccessStatusCode();
            var jsonRoles = await result.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<List<string>>(jsonRoles);
        }
    }
}
