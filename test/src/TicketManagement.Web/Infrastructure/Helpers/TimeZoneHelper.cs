using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NodaTime;
using TicketManagement.Web.Clients;

namespace TicketManagement.Web.Infrastructure.Helpers
{
    /// <summary>
    /// Provides date and time conversion capabilities.
    /// </summary>
    public class TimeZoneHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserClient _userClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZoneHelper"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Http context accessor for getting current user.</param>
        /// <param name="userClient">User client.</param>
        public TimeZoneHelper(IHttpContextAccessor httpContextAccessor, IUserClient userClient)
        {
            _httpContextAccessor = httpContextAccessor;
            _userClient = userClient;
        }

        /// <summary>
        /// Converts date and time from utc to local.
        /// </summary>
        /// <param name="utcDateTime">Utc date and time.</param>
        /// <returns>Date and time converted to local.</returns>
        public async Task<DateTime> ConvertToLocalAsync(DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Unspecified)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }

            if (utcDateTime.Kind != DateTimeKind.Local)
            {
                var localTimeZone = await GetUserTimeZone();

                return Instant.FromDateTimeUtc(utcDateTime).InZone(localTimeZone).ToDateTimeUnspecified();
            }
            else
            {
                return utcDateTime;
            }
        }

        /// <summary>
        /// Converts date and time from local to utc.
        /// </summary>
        /// <param name="localDateTime">Local date and time.</param>
        /// <returns>Date and time converted to utc.</returns>
        public async Task<DateTime> ConvertToUtcAsync(DateTime localDateTime)
        {
            if (localDateTime.Kind == DateTimeKind.Unspecified)
            {
                localDateTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Local);
            }

            if (localDateTime.Kind != DateTimeKind.Utc)
            {
                var localTimeZone = await GetUserTimeZone();
                var localtime = LocalDateTime.FromDateTime(localDateTime);
                var zonedtime = localtime.InZoneLeniently(localTimeZone);

                return zonedtime.ToInstant().InZone(localTimeZone).ToDateTimeUtc();
            }
            else
            {
                return localDateTime;
            }
        }

        /// <summary>
        /// Gets current user time zone.
        /// </summary>
        /// <returns>Current user time zone.</returns>
        private async Task<DateTimeZone> GetUserTimeZone()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return DateTimeZoneProviders.Tzdb.GetSystemDefault();
            }

            var currentUser = await _userClient.Get(userId);
            var timeZoneId = currentUser?.TimeZoneId ?? TimeZoneInfo.Local.Id;
            return DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneId)
                ?? DateTimeZoneProviders.Tzdb.GetSystemDefault();
        }
    }
}
