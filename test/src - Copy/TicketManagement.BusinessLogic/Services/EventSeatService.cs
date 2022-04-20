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
    /// Responsible for proxy all event seat repository calls with validation logic.
    /// </summary>
    internal class EventSeatService : BaseService<EventSeatDto>, IEventSeatService
    {
        private readonly IRepository<EventSeat> _eventSeatRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSeatService"/> class.
        /// </summary>
        /// <param name="eventAreaRepository">Event area repository.</param>
        /// <param name="eventSeatRepository">Event seat repository.</param>
        /// <param name="mapper">Mapper.</param>
        public EventSeatService(IRepository<EventArea> eventAreaRepository,
            IRepository<EventSeat> eventSeatRepository, IMapper mapper)
        {
            _eventSeatRepository = eventSeatRepository;
            _mapper = mapper;
            Validator = new EventSeatValidator(eventAreaRepository, eventSeatRepository);
        }

        /// <summary>
        /// Gets or sets validator for event seat.
        /// </summary>
        protected override AbstractValidator<EventSeatDto> Validator { get; set; }

        /// <summary>
        /// Creates event seat with validation.
        /// </summary>
        /// <param name="eventSeatDto">Event seat dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> CreateEventSeatAsync(EventSeatDto eventSeatDto)
        {
            var result = await TryValidateAsync(eventSeatDto, "Create");
            if (result.IsValid)
            {
                var eventSeat = _mapper.Map<EventSeatDto, EventSeat>(eventSeatDto);
                eventSeatDto.Id = await _eventSeatRepository.CreateAsync(eventSeat);
            }

            return result;
        }

        /// <summary>
        /// Updates seat with validation.
        /// </summary>
        /// <param name="eventSeatDto">Event seat dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> UpdateEventSeatAsync(EventSeatDto eventSeatDto)
        {
            var result = await TryValidateAsync(eventSeatDto, "Update");
            if (result.IsValid)
            {
                var eventSeat = _mapper.Map<EventSeatDto, EventSeat>(eventSeatDto);
                await _eventSeatRepository.UpdateAsync(eventSeat);
            }

            return result;
        }

        /// <summary>
        /// Deletes event seat with validation.
        /// </summary>
        /// <param name="eventSeatDto">Event seat dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> DeleteEventSeatAsync(EventSeatDto eventSeatDto)
        {
            var result = await TryValidateAsync(eventSeatDto, "Delete");
            if (result.IsValid)
            {
                var eventSeat = _mapper.Map<EventSeatDto, EventSeat>(eventSeatDto);
                await _eventSeatRepository.DeleteAsync(eventSeat);
            }

            return result;
        }

        /// <summary>
        /// Gets event seat from data storage by specified id.
        /// </summary>
        /// <param name="eventSeatId">Specified id of the event seat.</param>
        /// <returns>Event seat dto if the operation was successful otherwise null.</returns>
        public async Task<EventSeatDto> GetEventSeatAsync(int eventSeatId)
        {
            EventSeatDto eventSeatDto = null;
            var eventSeat = await _eventSeatRepository.GetByIdAsync(eventSeatId);
            if (eventSeat != null)
            {
                eventSeatDto = _mapper.Map<EventSeat, EventSeatDto>(eventSeat);
            }

            return eventSeatDto;
        }

        /// <summary>
        /// Gets all event seats from data storage.
        /// </summary>
        /// <returns>Event seat list if the operation was successful otherwise empty event seat list.</returns>
        public List<EventSeatDto> GetEventSeats()
        {
            var eventSeats = _eventSeatRepository.GetAll();
            List<EventSeatDto> eventSeatsDto = _mapper
                .Map<IQueryable<EventSeat>, List<EventSeatDto>>(eventSeats) ?? new List<EventSeatDto>();

            return eventSeatsDto;
        }

        /// <summary>
        /// Gets specified event seats.
        /// </summary>
        /// <returns>Specified Event seat list if the operation was successful otherwise empty event seat list.</returns>
        public List<EventSeatDto> GetEventSeatsBy(Expression<Func<EventSeat, bool>> predicate)
        {
            var eventSeats = _eventSeatRepository.GetAll().Where(predicate);
            List<EventSeatDto> eventSeatsDto = _mapper
                .Map<IQueryable<EventSeat>, List<EventSeatDto>>(eventSeats) ?? new List<EventSeatDto>();

            return eventSeatsDto;
        }
    }
}
