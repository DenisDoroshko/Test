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
    /// Responsible for proxy all seat repository calls with validation logic.
    /// </summary>
    public interface ISeatService
    {
        /// <summary>
        /// Creates seat with validation.
        /// </summary>
        /// <param name="seatDto">Seat dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> CreateSeatAsync(SeatDto seatDto);

        /// <summary>
        /// Updates seat with validation.
        /// </summary>
        /// <param name="seatDto">Seat dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> UpdateSeatAsync(SeatDto seatDto);

        /// <summary>
        /// Deletes seat with validation.
        /// </summary>
        /// <param name="seatDto">Seat dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> DeleteSeatAsync(SeatDto seatDto);

        /// <summary>
        /// Gets seat from data storage by specified id.
        /// </summary>
        /// <param name="seatId">Specified id of the seat.</param>
        /// <returns>Seat dto if the operation was successful otherwise null.</returns>
        Task<SeatDto> GetSeatAsync(int seatId);

        /// <summary>
        /// Gets all seats from data storage.
        /// </summary>
        /// <returns>Seat list if the operation was successful otherwise empty seat list.</returns>
        List<SeatDto> GetSeats();

        /// <summary>
        /// Gets specified seats.
        /// </summary>
        /// <returns>Specified Seat list if the operation was successful otherwise empty seat list.</returns>
        List<SeatDto> GetSeatsBy(Expression<Func<Seat, bool>> predicate);
    }
}
