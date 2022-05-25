using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.EventManagerApi.Models;
using TicketManagement.EventManagerApi.Services.Interfaces;

namespace TicketManagement.EventManagerApi.Services
{
    /// <summary>
    /// Responsible for proxy all event repository calls with validation logic.
    /// </summary>
    internal class EventService : IEventService
    {
        private readonly AbstractValidator<EventModel> _validator;
        private readonly IImageProcesser _imageProcesser;
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Venue> _venueRepositoty;
        private readonly IRepository<Layout> _layoutRepositoty;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventService"/> class.
        /// </summary>
        /// <param name="imageProcesser">Image processer.</param>
        /// <param name="venueRepository">Venue repository.</param>
        /// <param name="layoutRepository">Layout repository.</param>
        /// <param name="eventRepository">Event repository.</param>
        /// <param name="areaRepository">Area repository.</param>
        /// <param name="seatRepository">Seat repository.</param>
        /// <param name="eventAreaRepository">Event area repository.</param>
        /// <param name="eventSeatRepository">Event seat repository.</param>
        /// <param name="mapper">Mapper.</param>
#pragma warning disable S107 // Methods should not have too many parameters
        public EventService(IImageProcesser imageProcesser, IRepository<Venue> venueRepository, IRepository<Layout> layoutRepository,
            IRepository<Event> eventRepository,
            IRepository<Area> areaRepository, IRepository<Seat> seatRepository,
            IRepository<EventArea> eventAreaRepository, IRepository<EventSeat> eventSeatRepository, IMapper mapper)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            _imageProcesser = imageProcesser;
            _eventRepository = eventRepository;
            _venueRepositoty = venueRepository;
            _layoutRepositoty = layoutRepository;
            _mapper = mapper;
            _validator = new EventValidator(layoutRepository, eventRepository, areaRepository, seatRepository,
                eventAreaRepository, eventSeatRepository);
        }

        /// <inheritdoc />
        public async Task<ValidationResult> CreateEventAsync(EventModel eventModel)
        {
            var result = await TryValidateAsync(eventModel, "Create");
            if (result.IsValid)
            {
                var eventEntity = _mapper.Map<EventModel, Event>(eventModel);
                eventModel.Id = await _eventRepository.CreateAsync(eventEntity);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<ValidationResult> UpdateEventAsync(EventModel eventModel)
        {
            var result = await TryValidateAsync(eventModel, "Update");
            if (!result.IsValid)
            {
                return result;
            }

            var eventEntity = _mapper.Map<EventModel, Event>(eventModel);
            var oldEvent = await _eventRepository.GetByIdAsync(eventModel.Id);
            if (oldEvent != null && oldEvent.LayoutId != eventModel.LayoutId)
            {
                await _eventRepository.DeleteAsync(eventEntity);
                eventModel.Id = await _eventRepository.CreateAsync(eventEntity);
            }
            else
            {
                await _eventRepository.UpdateAsync(eventEntity);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<ValidationResult> DeleteEventAsync(EventModel eventModel)
        {
            var result = await TryValidateAsync(eventModel, "Delete");
            if (result.IsValid)
            {
                var eventEntity = _mapper.Map<EventModel, Event>(eventModel);
                await _eventRepository.DeleteAsync(eventEntity);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<EventModel> GetEventAsync(int eventModelId)
        {
            EventModel eventModel = null;
            var eventEntity = await _eventRepository.GetByIdAsync(eventModelId);
            var eventLayout = await _layoutRepositoty.GetByIdAsync(eventEntity != null ? eventEntity.LayoutId : 0);
            var eventVenue = await _venueRepositoty.GetByIdAsync(eventLayout != null ? eventLayout.VenueId : 0);
            if (eventEntity != null && eventLayout != null && eventVenue != null)
            {
                eventModel = new EventModel
                {
                    Id = eventEntity.Id,
                    Name = eventEntity.Name,
                    Description = eventEntity.Description,
                    Start = eventEntity.Start,
                    Finish = eventEntity.Finish,
                    LayoutId = eventEntity.LayoutId,
                    Image = await _imageProcesser.GetBase64Image(eventEntity.Image),
                    Address = eventVenue.Address,
                    Width = eventLayout.Width,
                    Height = eventLayout.Height,
                };
            }

            return eventModel;
        }

        /// <inheritdoc />
        public List<EventModel> GetEvents()
        {
            var events = _eventRepository.GetAll();
            var eventsDto = _mapper.Map<IQueryable<Event>, List<EventModel>>(events) ?? new List<EventModel>();

            return eventsDto;
        }

        /// <inheritdoc />
        public List<EventModel> GetEventsBy(Expression<Func<Event, bool>> predicate)
        {
            var events = _eventRepository.GetAll().Where(predicate);
            var eventsDto = _mapper.Map<IQueryable<Event>, List<EventModel>>(events) ?? new List<EventModel>();

            return eventsDto;
        }

        private async Task<ValidationResult> TryValidateAsync(EventModel eventModel, string ruleSetName)
        {
            ValidationResult result = null;
            try
            {
                result = await _validator.ValidateAsync(eventModel, options => options.IncludeRuleSets(ruleSetName));
            }
            catch (Exception exception)
            {
                result ??= new ValidationResult(new List<ValidationFailure>());
                result.Errors.Add(new ValidationFailure("", exception.Message));
            }

            return result;
        }
    }
}
