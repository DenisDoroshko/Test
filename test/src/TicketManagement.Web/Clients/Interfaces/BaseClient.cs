using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using TicketManagement.Web.JwtTokenAuth;

namespace TicketManagement.Web.Clients
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

            httpClient.DefaultRequestHeaders.AcceptLanguage
                .Add(new StringWithQualityHeaderValue(CultureInfo.CurrentCulture.TwoLetterISOLanguageName));
            var token = httpContextAccessor.HttpContext.Request.Cookies[JwtTokenConstants.JwtCookieKey];
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
