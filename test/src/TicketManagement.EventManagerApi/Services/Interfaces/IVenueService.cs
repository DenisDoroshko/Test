using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;
using TicketManagement.EventManagerApi.Models;

namespace TicketManagement.EventManagerApi.Services.Interfaces
{
    /// <summary>
    /// Responsible for proxy venue repository calls.
    /// </summary>
    public interface IVenueService
    {
        /// <summary>
        /// Gets all venues.
        /// </summary>
        /// <returns>All venues.</returns>
        List<VenueModel> GetVenues();

        /// <summary>
        /// Gets specified venues.
        /// </summary>
        /// <returns>Specified Venue list if the operation was successful otherwise empty venue list.</returns>
        List<VenueModel> GetVenuesBy(Expression<Func<Venue, bool>> predicate);
    }
}
