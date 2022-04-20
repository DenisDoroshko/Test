using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace TicketManagement.ReactWeb.Clients
{
    /// <summary>
    /// Represents base class for clients.
    /// </summary>
    public class BaseClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClient"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Http token accessor.</param>
        /// <param name="httpClient">Http client.</param>
        public BaseClient(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            SetHeaders(httpContextAccessor, httpClient);
        }

        /// <summary>
        /// If exist sets token to cookies.
        /// </summary>
        /// <param name="httpContextAccessor">Http context accessor.</param>
        /// <param name="httpClient">Http client.</param>
        private void SetHeaders(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            if (httpContextAccessor.HttpContext == null)
            {
                return;
            }

            var languageHeader = httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];
            if (languageHeader.Any())
            {
                httpClient.DefaultRequestHeaders.AcceptLanguage
                .Add(new StringWithQualityHeaderValue(languageHeader.ToString().Split(',').FirstOrDefault()));
            }

            var authorizeHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (authorizeHeader.Any())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", (string)authorizeHeader);
            }
        }
    }
}
