using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validators;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Services
{
    /// <summary>
    /// Responsible for proxy all event repository calls with validation logic.
    /// </summary>
    internal class EventService : BaseService<EventDto>, IEventService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventService"/> class.
        /// </summary>
        /// <param name="layoutRepository">Layout repository.</param>
        /// <param name="eventRepository">Event repository.</param>
        /// <param name="areaRepository">Area repository.</param>
        /// <param name="seatRepository">Seat repository.</param>
        /// <param name="eventAreaRepository">Event area repository.</param>
        /// <param name="eventSeatRepository">Event seat repository.</param>
        /// <param name="mapper">Mapper.</param>
        public EventService(IRepository<Layout> layoutRepository, IRepository<Event> eventRepository,
            IRepository<Area> areaRepository, IRepository<Seat> seatRepository,
            IRepository<EventArea> eventAreaRepository, IRepository<EventSeat> eventSeatRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
            Validator = new EventValidator(layoutRepository, eventRepository, areaRepository, seatRepository,
                eventAreaRepository, eventSeatRepository);
        }

        /// <summary>
        /// Gets or sets validator for event.
        /// </summary>
        protected override AbstractValidator<EventDto> Validator { get; set; }

        /// <summary>
        /// Creates event with validation.
        /// </summary>
        /// <param name="eventDto">Event dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> CreateEventAsync(EventDto eventDto)
        {
            var result = await TryValidateAsync(eventDto, "Create");
            if (result.IsValid)
            {
                var eventEntity = _mapper.Map<EventDto, Event>(eventDto);
                eventDto.Id = await _eventRepository.CreateAsync(eventEntity);
            }

            return result;
        }

        /// <summary>
        /// Updates event with validation.
        /// </summary>
        /// <param name="eventDto">Event dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> UpdateEventAsync(EventDto eventDto)
        {
            var result = await TryValidateAsync(eventDto, "Update");
            if (result.IsValid)
            {
                var eventEntity = _mapper.Map<EventDto, Event>(eventDto);
                var oldEvent = await _eventRepository.GetByIdAsync(eventDto.Id);
                if (oldEvent != null && oldEvent.LayoutId != eventDto.LayoutId)
                {
                    await _eventRepository.DeleteAsync(eventEntity);
                    eventDto.Id = await _eventRepository.CreateAsync(eventEntity);
                }
                else
                {
                    await _eventRepository.UpdateAsync(eventEntity);
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes event with validation.
        /// </summary>
        /// <param name="eventDto">Event dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> DeleteEventAsync(EventDto eventDto)
        {
            var result = await TryValidateAsync(eventDto, "Delete");
            if (result.IsValid)
            {
                var eventEntity = _mapper.Map<EventDto, Event>(eventDto);
                await _eventRepository.DeleteAsync(eventEntity);
            }

            return result;
        }

        /// <summary>
        /// Gets event from data storage by specified id.
        /// </summary>
        /// <param name="eventDtoId">Specified id of the event.</param>
        /// <returns>Event dto if the operation was successful otherwise null.</returns>
        public async Task<EventDto> GetEventAsync(int eventDtoId)
        {
            EventDto eventDto = null;
            var eventEntity = await _eventRepository.GetByIdAsync(eventDtoId);
            if (eventEntity != null)
            {
                eventDto = _mapper.Map<Event, EventDto>(eventEntity);
            }

            return eventDto;
        }

        /// <summary>
        /// Gets all events from data storage.
        /// </summary>
        /// <returns>Event list if the operation was successful otherwise empty event list.</returns>
        public List<EventDto> GetEvents()
        {
            var events = _eventRepository.GetAll();
            var eventsDto = _mapper.Map<IQueryable<Event>, List<EventDto>>(events) ?? new List<EventDto>();

            return eventsDto;
        }

        /// <summary>
        /// Get selected events.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Events list.</returns>
        public List<EventDto> GetEventsBy(Expression<Func<Event, bool>> predicate)
        {
            var events = _eventRepository.GetAll().Where(predicate);
            var eventsDto = _mapper.Map<IQueryable<Event>, List<EventDto>>(events) ?? new List<EventDto>();

            return eventsDto;
        }
    }
}
