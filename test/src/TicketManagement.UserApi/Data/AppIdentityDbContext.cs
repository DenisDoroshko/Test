using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketManagement.UserApi.Models;

namespace TicketManagement.UserApi.Data
{
    /// <summary>
    /// Provides access to identity tables.
    /// </summary>
    public class AppIdentityDbContext : IdentityDbContext<UserModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppIdentityDbContext"/> class.
        /// </summary>
        /// <param name="options">Database context options.</param>
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            : base(options)
        {
        }
    }
}
