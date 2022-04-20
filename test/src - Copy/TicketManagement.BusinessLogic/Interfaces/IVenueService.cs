using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    /// <summary>
    /// Responsible for proxy all venue repository calls with validation logic.
    /// </summary>
    public interface IVenueService
    {
        /// <summary>
        /// Creates venue with validation.
        /// </summary>
        /// <param name="venueDto">Venue dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> CreateVenueAsync(VenueDto venueDto);

        /// <summary>
        /// Updates venue with validation.
        /// </summary>
        /// <param name="venueDto">Venue dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> UpdateVenueAsync(VenueDto venueDto);

        /// <summary>
        /// Deletes venue with validation.
        /// </summary>
        /// <param name="venueDto">Venue dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> DeleteVenueAsync(VenueDto venueDto);

        /// <summary>
        /// Gets venue from data storage by specified id.
        /// </summary>
        /// <param name="venueId">Specified id of the venue.</param>
        /// <returns>Venue dto if the operation was successful otherwise null.</returns>
        Task<VenueDto> GetVenueAsync(int venueId);

        /// <summary>
        /// Gets all venues from data storage.
        /// </summary>
        /// <returns>Venue list if the operation was successful otherwise empty venue list.</returns>
        List<VenueDto> GetVenues();

        /// <summary>
        /// Gets specified venues.
        /// </summary>
        /// <returns>Specified Venue list if the operation was successful otherwise empty venue list.</returns>
        List<VenueDto> GetVenuesBy(Expression<Func<Venue, bool>> predicate);
    }
}
