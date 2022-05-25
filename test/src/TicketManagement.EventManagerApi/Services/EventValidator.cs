using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.EventManagerApi.Models;

namespace TicketManagement.EventManagerApi.Services
{
    /// <summary>
    /// Validator for the event with different rule sets.
    /// </summary>
    internal class EventValidator : AbstractValidator<EventModel>
    {
        private readonly IRepository<Layout> _layoutRepository;

        private readonly IRepository<Event> _eventRepository;

        private readonly IRepository<Area> _areaRepository;

        private readonly IRepository<Seat> _seatRepository;

        private readonly IRepository<EventArea> _eventAreaRepository;

        private readonly IRepository<EventSeat> _eventSeatRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventValidator"/> class.
        /// </summary>
        /// <param name="layoutRepository">Layout repository.</param>
        /// <param name="eventRepository">Event repository.</param>
        /// <param name="areaRepository">Area repository.</param>
        /// <param name="seatRepository">Seat repository.</param>
        /// <param name="eventAreaRepository">Event area repository.</param>
        /// <param name="eventSeatRepository">Event seat repository.</param>
        public EventValidator(IRepository<Layout> layoutRepository,
            IRepository<Event> eventRepository, IRepository<Area> areaRepository,
            IRepository<Seat> seatRepository, IRepository<EventArea> eventAreaRepository, IRepository<EventSeat> eventSeatRepository)
        {
            _layoutRepository = layoutRepository;
            _eventRepository = eventRepository;
            _areaRepository = areaRepository;
            _seatRepository = seatRepository;
            _eventAreaRepository = eventAreaRepository;
            _eventSeatRepository = eventSeatRepository;
            RuleSet("Create", () =>
            {
                RuleFor(e => e.LayoutId)
                    .MustAsync(async (layoutId, cancancellation) => await IsExistLayoutForEventAsync(layoutId))
                    .WithMessage("There is no layout for this event.");
                RuleFor(e => e.LayoutId)
                    .MustAsync(async (layoutId, cancancellation) => await IsExistAnySeatAsync(layoutId))
                    .WithMessage("Can not create event from layout that does not contain any seats.");
                RuleFor(e => e)
                    .MustAsync(async (e, cancancellation) => await IsNotExistDateDuringCreateAsync(e))
                    .WithMessage("Can not create event for the same venue in the same time.");
                RuleFor(e => e)
                    .Must(e => e.Start < e.Finish)
                    .WithMessage("Event finish can not be before event start.");
                RuleFor(e => e)
                    .Must(e => e.Start > DateTime.UtcNow && e.Finish > DateTime.UtcNow)
                    .WithMessage("Can not create event in the past.");
            });

            RuleSet("Update", () =>
            {
                RuleFor(e => e.Id)
                    .MustAsync(async (eventId, cancancellation) => await IsExistEventAsync(eventId))
                    .WithMessage("There is no event with this id.");
                RuleFor(e => e)
                    .MustAsync(async (e, cancancellation) => await IsNotExistDateDuringUpdateAsync(e))
                    .WithMessage("Can not update event for the same venue in the same time.");
                RuleFor(t => t)
                    .Must(t => t.Start < t.Finish)
                    .WithMessage("Event finish can not be before event start.");
                RuleFor(t => t)
                    .Must(t => t.Start > DateTime.UtcNow && t.Finish > DateTime.UtcNow)
                    .WithMessage("Can not update event in the past.");
                RuleFor(e => e).MustAsync(async (e, cancancellation) => await IsAvailalbleForChangeLayout(e))
                    .WithMessage("Can't change layout. There are unavalaible seats.");
                RuleFor(e => e.LayoutId)
                    .MustAsync(async (layoutId, cancancellation) => await IsExistLayoutForEventAsync(layoutId))
                    .WithMessage("There is no layout for this event.");
                RuleFor(e => e.LayoutId)
                    .MustAsync(async (layoutId, cancancellation) => await IsExistAnySeatAsync(layoutId))
                    .WithMessage("Can not update event from layout that does not contain any seats.");
            });

            RuleSet("Delete", () =>
            {
                RuleFor(e => e.Id)
                    .MustAsync(async (eventId, cancancellation) => await IsExistEventAsync(eventId))
                    .WithMessage("There is no event with this id.");
                RuleFor(e => e.Id)
                    .Must(eventId => IsAllSeatsAvailable(eventId))
                    .WithMessage("There are unavalaible seats.");
            });
        }

        /// <summary>
        /// Checks if the layout for event exists.
        /// </summary>
        /// <param name="layoutId">Layout id.</param>
        /// <returns>True if layout for event exists otherwise false.</returns>
        private async Task<bool> IsExistLayoutForEventAsync(int layoutId)
        {
            return await _layoutRepository.GetByIdAsync(layoutId) != null;
        }

        /// <summary>
        /// Checks if the any seats exist in layout.
        /// </summary>
        /// <param name="layoutId">Layout id.</param>
        /// <returns>True if any seats exist otherwise false.</returns>
        private async Task<bool> IsExistAnySeatAsync(int layoutId)
        {
            var layout = await _layoutRepository.GetByIdAsync(layoutId);
            if (layout != null)
            {
                var areas = _areaRepository.GetAll().Where(a => a.LayoutId == layout.Id);
                var seats = _seatRepository.GetAll().Where(s => areas.Any(a => a.Id == s.AreaId));
                return seats.Any();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if all seats available during changing layout.
        /// </summary>
        /// <param name="eventModel">Event model.</param>
        /// <returns>True if any seats exist otherwise false.</returns>
        private async Task<bool> IsAvailalbleForChangeLayout(EventModel eventModel)
        {
            var oldEvent = await _eventRepository.GetByIdAsync(eventModel.Id);
            if (oldEvent != null && oldEvent.LayoutId != eventModel.LayoutId)
            {
                var areas = _eventAreaRepository.GetAll().Where(a => a.EventId == eventModel.Id);
                var seats = _eventSeatRepository.GetAll().Where(s => areas.Any(a => a.Id == s.EventAreaId));
                return seats.All(s => s.State == 0);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Сhecks if there is event with the same time and date in venue(for create rule).
        /// </summary>
        /// <param name="eventModel">Event.</param>
        /// <returns>True if not exist otherwise false.</returns>
        private async Task<bool> IsNotExistDateDuringCreateAsync(EventModel eventModel)
        {
            var layout = await _layoutRepository.GetByIdAsync(eventModel.LayoutId);
            if (layout != null)
            {
                var layouts = _layoutRepository.GetAll().Where(l => l.VenueId == layout.VenueId);
                var events = _eventRepository.GetAll().Where(e => layouts.Any(l => l.Id == e.LayoutId));
                return !events.Any() || events.All(item => item.Start > eventModel.Finish || eventModel.Start > item.Finish);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Сhecks if there is event with the same time and date in venue(for update rule).
        /// Allows the time and date to match for events with the same id.
        /// </summary>
        /// <param name="eventModel">Event.</param>
        /// <returns>True if not exist otherwise false.</returns>
        private async Task<bool> IsNotExistDateDuringUpdateAsync(EventModel eventModel)
        {
            var layout = await _layoutRepository.GetByIdAsync(eventModel.LayoutId);
            if (layout != null)
            {
                var layouts = _layoutRepository.GetAll().Where(l => l.VenueId == layout.VenueId);
                var events = _eventRepository.GetAll().Where(e => layouts.Any(l => l.Id == e.LayoutId) && e.Id!= eventModel.Id);
                return events.All(item => item.Start > eventModel.Finish || eventModel.Start > item.Finish);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the event  exists.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <returns>True if exists otherwise false.</returns>
        private async Task<bool> IsExistEventAsync(int eventId)
        {
            return await _eventRepository.GetByIdAsync(eventId) != null;
        }

        /// <summary>
        /// Checks if no one has purchased ticket yet.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <returns>True if all tickets available otherwise false.</returns>
        private bool IsAllSeatsAvailable(int eventId)
        {
            var query = from a in _eventAreaRepository.GetAll().Where(a => a.EventId == eventId)
            join s in _eventSeatRepository.GetAll() on a.Id equals s.EventAreaId
            select new { State = s.State };
            return query.All(t => t.State == 0);
        }
    }
}
