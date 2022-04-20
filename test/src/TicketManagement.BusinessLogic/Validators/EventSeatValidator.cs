using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validators
{
    /// <summary>
    /// Validator for event seat with different rule sets.
    /// </summary>
    public class EventSeatValidator : AbstractValidator<EventSeatDto>
    {
        private readonly IRepository<EventArea> _eventAreaRepository;

        private readonly IRepository<EventSeat> _eventSeatRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSeatValidator"/> class.
        /// </summary>
        /// <param name="eventAreaRepository">Event area repository.</param>
        /// <param name="eventSeatRepository">Event seat repository.</param>
        public EventSeatValidator(IRepository<EventArea> eventAreaRepository, IRepository<EventSeat> eventSeatRepository)
        {
            _eventAreaRepository = eventAreaRepository;
            _eventSeatRepository = eventSeatRepository;
            RuleSet("Create", () =>
            {
                RuleFor(s => s.EventAreaId)
                    .MustAsync(async (areaId, cancancellation) => await IsExistAreaForSeatAsync(areaId))
                    .WithMessage("There is no area for this event seat.");
                RuleFor(s => s)
                    .Must(s => IsUniqueRowAndNumberDuringCreate(s))
                    .WithMessage("Seat row and number should be unique for area.");
            });

            RuleSet("Update", () =>
            {
                RuleFor(s => s.Id)
                    .MustAsync(async (eventSeatId, cancancellation) => await IsExistSeatAsync(eventSeatId))
                    .WithMessage("There is no seat with this id.");
                RuleFor(s => s)
                    .Must(s => IsUniqueRowAndNumberDuringUpdate(s))
                    .WithMessage("Seat row and number should be unique for area.");
            });

            RuleSet("Delete", () =>
            {
                RuleFor(s => s.Id)
                    .MustAsync(async (eventSeatId, cancancellation) => await IsExistSeatAsync(eventSeatId))
                    .WithMessage("There is no seat with this id.");
            });
        }

        /// <summary>
        /// Checks if the event seat  exists.
        /// </summary>
        /// <param name="eventSeatId">Event seat id.</param>
        /// <returns>True if exists otherwise false.</returns>
        private async Task<bool> IsExistSeatAsync(int eventSeatId)
        {
            return await _eventSeatRepository.GetByIdAsync(eventSeatId) != null;
        }

        /// <summary>
        /// Checks if the event area for event seat exists.
        /// </summary>
        /// <param name="eventAreaId">Event area id.</param>
        /// <returns>True if event area for event seat exists otherwise false.</returns>
        private async Task<bool> IsExistAreaForSeatAsync(int eventAreaId)
        {
            return await _eventAreaRepository.GetByIdAsync(eventAreaId) != null;
        }

        /// <summary>
        /// Checks if the seat row and number unique(for create rule).
        /// </summary>
        /// <param name="eventSeat">Event seat.</param>
        /// <returns>True if unique otherwise false.</returns>
        private bool IsUniqueRowAndNumberDuringCreate(EventSeatDto eventSeat)
        {
            var loadedSeats = _eventSeatRepository.GetAll()
                .Where(item => item.EventAreaId == eventSeat.EventAreaId);
            return loadedSeats.All(item => item.Row != eventSeat.Row || item.Number != eventSeat.Number);
        }

        /// <summary>
        /// Checks if the seat row and number unique(for update rule).
        /// Allows the row and number to match for seats with the same id.
        /// </summary>
        /// <param name="eventSeat">Event seat.</param>
        /// <returns>True if unique otherwise false.</returns>
        private bool IsUniqueRowAndNumberDuringUpdate(EventSeatDto eventSeat)
        {
            var loadedSeats = _eventSeatRepository.GetAll()
                .Where(item => item.EventAreaId == eventSeat.EventAreaId);
            return loadedSeats.All(item => item.Row != eventSeat.Row || item.Number != eventSeat.Number
            || item.Id == eventSeat.Id);
        }
    }
}
