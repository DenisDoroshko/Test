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
    /// Responsible for proxy all event area repository calls with validation logic.
    /// </summary>
    internal class EventAreaService : BaseService<EventAreaDto>, IEventAreaService
    {
        private readonly IRepository<EventArea> _eventAreaRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAreaService"/> class.
        /// </summary>
        /// <param name="eventRepository">Event repository.</param>
        /// <param name="eventAreaRepository">Event area repository.</param>
        /// <param name="mapper">Mapper.</param>
        public EventAreaService(IRepository<Event> eventRepository, IRepository<EventArea> eventAreaRepository, IMapper mapper)
        {
            _eventAreaRepository = eventAreaRepository;
            _mapper = mapper;
            Validator = new EventAreaValidator(eventRepository, eventAreaRepository);
        }

        /// <summary>
        /// Gets or sets validator for event area.
        /// </summary>
        protected override AbstractValidator<EventAreaDto> Validator { get; set; }

        /// <summary>
        /// Creates event area with validation.
        /// </summary>
        /// <param name="eventAreaDto">Event area dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> CreateEventAreaAsync(EventAreaDto eventAreaDto)
        {
            var result = await TryValidateAsync(eventAreaDto, "Create");
            if (result.IsValid)
            {
                var eventArea = _mapper.Map<EventAreaDto, EventArea>(eventAreaDto);
                eventAreaDto.Id = await _eventAreaRepository.CreateAsync(eventArea);
            }

            return result;
        }

        /// <summary>
        /// Updates area with validation.
        /// </summary>
        /// <param name="eventAreaDto">Event area dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> UpdateEventAreaAsync(EventAreaDto eventAreaDto)
        {
            var result = await TryValidateAsync(eventAreaDto, "Update");
            if (result.IsValid)
            {
                var eventArea = _mapper.Map<EventAreaDto, EventArea>(eventAreaDto);
                await _eventAreaRepository.UpdateAsync(eventArea);
            }

            return result;
        }

        /// <summary>
        /// Deletes area with validation.
        /// </summary>
        /// <param name="eventAreaDto">Event area dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> DeleteEventAreaAsync(EventAreaDto eventAreaDto)
        {
            var result = await TryValidateAsync(eventAreaDto, "Delete");
            if (result.IsValid)
            {
                var eventArea = _mapper.Map<EventAreaDto, EventArea>(eventAreaDto);
                await _eventAreaRepository.DeleteAsync(eventArea);
            }

            return result;
        }

        /// <summary>
        /// Gets area from data storage by specified id.
        /// </summary>
        /// <param name="eventAreaId">Specified id of the event area.</param>
        /// <returns>Event area dto if the operation was successful otherwise null.</returns>
        public async Task<EventAreaDto> GetEventAreaAsync(int eventAreaId)
        {
            EventAreaDto eventAreaDto = null;
            var eventArea = await _eventAreaRepository.GetByIdAsync(eventAreaId);
            if (eventArea != null)
            {
                eventAreaDto = _mapper.Map<EventArea, EventAreaDto>(eventArea);
            }

            return eventAreaDto;
        }

        /// <summary>
        /// Gets all event areas from data storage.
        /// </summary>
        /// <returns>Event area list if the operation was successful otherwise empty event area list.</returns>
        public List<EventAreaDto> GetEventAreas()
        {
            var eventAreas = _eventAreaRepository.GetAll();
            var eventAreasDto = _mapper
                .Map<IQueryable<EventArea>, List<EventAreaDto>>(eventAreas) ?? new List<EventAreaDto>();

            return eventAreasDto;
        }

        /// <summary>
        /// Gets specified event areas.
        /// </summary>
        /// <returns>Specified Event area list if the operation was successful otherwise empty event area list.</returns>
        public List<EventAreaDto> GetEventAreasBy(Expression<Func<EventArea, bool>> predicate)
        {
            var eventAreas = _eventAreaRepository.GetAll().Where(predicate);
            var eventAreasDto = _mapper
                .Map<IQueryable<EventArea>, List<EventAreaDto>>(eventAreas) ?? new List<EventAreaDto>();

            return eventAreasDto;
        }
    }
}
