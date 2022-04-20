using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validators
{
    /// <summary>
    /// Validator for the seat with different rule sets.
    /// </summary>
    public class SeatValidator : AbstractValidator<SeatDto>
    {
        private readonly IRepository<Area> _areaRepository;

        private readonly IRepository<Seat> _seatRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeatValidator"/> class.
        /// </summary>
        /// <param name="areaRepository">Event area repository.</param>
        /// <param name="seatRepository">Event seat repository.</param>
        public SeatValidator(IRepository<Area> areaRepository, IRepository<Seat> seatRepository)
        {
            _areaRepository = areaRepository;
            _seatRepository = seatRepository;
            RuleSet("Create", () =>
            {
                RuleFor(s => s.AreaId)
                    .MustAsync(async (areaId, cancancellation) => await IsExistAreaForSeatAsync(areaId))
                    .WithMessage("There is no area for this seat.");
                RuleFor(s => s)
                    .Must(s => IsCorrectRowAndNumberDuringCreate(s))
                    .WithMessage("Seat row and number should be unique for area.");
            });

            RuleSet("Update", () =>
            {
                RuleFor(s => s.Id)
                    .MustAsync(async (seatId, cancancellation) => await IsExistSeatAsync(seatId))
                    .WithMessage("There is no seat with this id.");
                RuleFor(s => s)
                    .Must(s => IsCorrectRowAndNumberDuringUpdate(s))
                    .WithMessage("Seat row and number should be unique for area.");
            });

            RuleSet("Delete", () =>
            {
                RuleFor(s => s.Id)
                    .MustAsync(async (seatId, cancancellation) => await IsExistSeatAsync(seatId))
                    .WithMessage("There is no seat with this id.");
            });
        }

        /// <summary>
        /// Checks if the seat  exists.
        /// </summary>
        /// <param name="seatId">Seat id.</param>
        /// <returns>True if exists otherwise false.</returns>
        private async Task<bool> IsExistSeatAsync(int seatId)
        {
            return await _seatRepository.GetByIdAsync(seatId) != null;
        }

        /// <summary>
        /// Checks if the area for seat exists.
        /// </summary>
        /// <param name="areaId">Area id.</param>
        /// <returns>True if area for seat exists otherwise false.</returns>
        private async Task<bool> IsExistAreaForSeatAsync(int areaId)
        {
            return await _areaRepository.GetByIdAsync(areaId) != null;
        }

        /// <summary>
        /// Checks if the seat row and number unique(for create rule).
        /// </summary>
        /// <param name="seat">Seat.</param>
        /// <returns>True if unique otherwise false.</returns>
        private bool IsCorrectRowAndNumberDuringCreate(SeatDto seat)
        {
            var loadedSeats = _seatRepository.GetAll().Where(item => item.AreaId == seat.AreaId);
            return loadedSeats.All(item => item.Row != seat.Row || item.Number != seat.Number);
        }

        /// <summary>
        /// Checks if the seat row and number unique(for update rule).
        /// Allows the row and number to match for seats with the same id.
        /// </summary>
        /// <param name="seat">Seat.</param>
        /// <returns>True if unique otherwise false.</returns>
        private bool IsCorrectRowAndNumberDuringUpdate(SeatDto seat)
        {
            var loadedSeats = _seatRepository.GetAll().Where(item => item.AreaId == seat.AreaId);
            return loadedSeats.All(item => item.Row != seat.Row || item.Number != seat.Number || item.Id == seat.Id);
        }
    }
}
