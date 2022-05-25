using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validators
{
    /// <summary>
    /// Validator for the event area with different rule sets.
    /// </summary>
    public class EventAreaValidator : AbstractValidator<EventAreaDto>
    {
        private readonly IRepository<Event> _eventRepository;

        private readonly IRepository<EventArea> _eventAreaRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAreaValidator"/> class.
        /// </summary>
        /// <param name="eventRepository">Event repository.</param>
        /// <param name="eventAreaRepository">Event area repository.</param>
        public EventAreaValidator(IRepository<Event> eventRepository, IRepository<EventArea> eventAreaRepository)
        {
            _eventRepository = eventRepository;
            _eventAreaRepository = eventAreaRepository;
            RuleSet("Create", () =>
            {
                RuleFor(a => a.EventId)
                    .MustAsync(async (layoutId, cancancellation) => await IsExistEventForAreaAsync(layoutId))
                    .WithMessage("There is no event for this event area.");
                RuleFor(a => a)
                    .Must(a => IsUniqueDescriptionDuringCreate(a))
                    .WithMessage("Area description should be unique in event.");
            });

            RuleSet("Update", () =>
            {
                RuleFor(a => a.Id)
                    .MustAsync(async (eventAreaId, cancancellation) => await IsExistAreaAsync(eventAreaId))
                    .WithMessage("There is no event area with this id.");
                RuleFor(a => a)
                    .Must(a => IsUniqueDescriptionDuringUpdate(a))
                    .WithMessage("Area description should be unique in event.");
            });

            RuleSet("Delete", () =>
            {
                RuleFor(a => a.Id)
                    .MustAsync(async (eventAreaId, cancancellation) => await IsExistAreaAsync(eventAreaId))
                    .WithMessage("There is no event area with this id.");
            });
        }

        /// <summary>
        /// Checks if the event for area exists.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <returns>True if event for area exists otherwise false.</returns>
        private async Task<bool> IsExistEventForAreaAsync(int eventId)
        {
            return await _eventRepository.GetByIdAsync(eventId) != null;
        }

        /// <summary>
        /// Checks if the area description unique(for create rule).
        /// </summary>
        /// <param name="area">Event area.</param>
        /// <returns>True if unique otherwise false.</returns>
        private bool IsUniqueDescriptionDuringCreate(EventAreaDto area)
        {
            var loadedAreas = _eventAreaRepository.GetAll().Where(item => item.EventId == area.EventId);
            return loadedAreas.All(item => item.Description != area.Description);
        }

        /// <summary>
        /// Checks if the area description unique(for update rule).
        /// Allows the description to match for areas with the same id.
        /// </summary>
        /// <param name="area">Event area.</param>
        /// <returns>True if unique otherwise false.</returns>
        private bool IsUniqueDescriptionDuringUpdate(EventAreaDto area)
        {
            var loadedAreas = _eventAreaRepository.GetAll().Where(item => item.EventId == area.EventId);
            return loadedAreas.All(item => item.Description != area.Description || item.Id == area.Id);
        }

        /// <summary>
        /// Checks if the event area  exists.
        /// </summary>
        /// <param name="eventAreaId">Event area id.</param>
        /// <returns>True if exists otherwise false.</returns>
        private async Task<bool> IsExistAreaAsync(int eventAreaId)
        {
            return await _eventAreaRepository.GetByIdAsync(eventAreaId) != null;
        }
    }
}
