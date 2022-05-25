using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace TicketManagement.Web.Infrastructure.Extensions
{
    /// <summary>
    /// Provides methods for working with session.
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Serealizes values to session by given key.
        /// </summary>
        /// <param name="session">Session.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value to serialize.</param>
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        /// <summary>
        /// Deserializes value from session by given key.
        /// </summary>
        /// <typeparam name="T">Saved value type.</typeparam>
        /// <param name="session">Session.</param>
        /// <param name="key">Key.</param>
        /// <returns>Value from session by given key.</returns>
        public static T GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);
            return sessionData == null
            ? default(T) : JsonSerializer.Deserialize<T>(sessionData);
        }
    }
}
