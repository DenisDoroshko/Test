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
    /// Responsible for proxy all seat repository calls with validation logic.
    /// </summary>
    internal class SeatService : BaseService<SeatDto>, ISeatService
    {
        private readonly IRepository<Area> _areaRepository;

        private readonly IRepository<Seat> _seatRepository;

        private readonly IRepository<Event> _eventRepository;

        private readonly IRepository<EventArea> _eventAreaRepository;

        private readonly IRepository<EventSeat> _eventSeatRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeatService"/> class.
        /// </summary>
        /// <param name="areaRepository">Area repository.</param>
        /// <param name="seatRepository">Seat repository.</param>
        /// <param name="eventRepository">Event repository.</param>
        /// <param name="eventAreaRepository">Event area repository.</param>
        /// <param name="eventSeatRepository">Event seat repository.</param>
        /// <param name="mapper">Mapper.</param>
        public SeatService(IRepository<Area> areaRepository, IRepository<Seat> seatRepository,
            IRepository<Event> eventRepository, IRepository<EventArea> eventAreaRepository,
            IRepository<EventSeat> eventSeatRepository, IMapper mapper)
        {
            _areaRepository = areaRepository;
            _seatRepository = seatRepository;
            _eventRepository = eventRepository;
            _eventAreaRepository = eventAreaRepository;
            _eventSeatRepository = eventSeatRepository;
            _mapper = mapper;
            Validator = new SeatValidator(areaRepository, seatRepository);
        }

        /// <summary>
        /// Gets or sets validator for seat.
        /// </summary>
        protected override AbstractValidator<SeatDto> Validator { get; set; }

        /// <summary>
        /// Creates seat with validation.
        /// </summary>
        /// <param name="seatDto">Seat dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> CreateSeatAsync(SeatDto seatDto)
        {
            var result = await TryValidateAsync(seatDto, "Create");
            if (result.IsValid)
            {
                await RecreateEvents(seatDto.AreaId);

                var seat = _mapper.Map<SeatDto, Seat>(seatDto);
                seatDto.Id = await _seatRepository.CreateAsync(seat);
            }

            return result;
        }

        /// <summary>
        /// Updates seat with validation.
        /// </summary>
        /// <param name="seatDto">Seat dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> UpdateSeatAsync(SeatDto seatDto)
        {
            var result = await TryValidateAsync(seatDto, "Update");
            if (result.IsValid)
            {
                await RecreateEvents(seatDto.AreaId);

                var seat = _mapper.Map<SeatDto, Seat>(seatDto);
                await _seatRepository.UpdateAsync(seat);
            }

            return result;
        }

        /// <summary>
        /// Deletes seat with validation.
        /// </summary>
        /// <param name="seatDto">Seat dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> DeleteSeatAsync(SeatDto seatDto)
        {
            var result = await TryValidateAsync(seatDto, "Delete");
            if (result.IsValid)
            {
                await RecreateEvents(seatDto.AreaId);

                var seat = _mapper.Map<SeatDto, Seat>(seatDto);
                await _seatRepository.DeleteAsync(seat);
            }

            return result;
        }

        /// <summary>
        /// Gets seat from data storage by specified id.
        /// </summary>
        /// <param name="seatId">Specified id of the seat.</param>
        /// <returns>Seat dto if the operation was successful otherwise null.</returns>
        public async Task<SeatDto> GetSeatAsync(int seatId)
        {
            var seat = await _seatRepository.GetByIdAsync(seatId);
            SeatDto seatDto = null;
            if (seat != null)
            {
                seatDto = _mapper.Map<Seat, SeatDto>(seat);
            }

            return seatDto;
        }

        /// <summary>
        /// Gets all seats from data storage.
        /// </summary>
        /// <returns>Seat list if the operation was successful otherwise empty seat list.</returns>
        public List<SeatDto> GetSeats()
        {
            var seats = _seatRepository.GetAll();
            var seatsDto = _mapper.Map<IQueryable<Seat>, List<SeatDto>>(seats) ?? new List<SeatDto>();

            return seatsDto;
        }

        /// <summary>
        /// Gets specified seats.
        /// </summary>
        /// <returns>Specified Seat list if the operation was successful otherwise empty seat list.</returns>
        public List<SeatDto> GetSeatsBy(Expression<Func<Seat, bool>> predicate)
        {
            var seats = _seatRepository.GetAll().Where(predicate);
            var seatsDto = _mapper.Map<IQueryable<Seat>, List<SeatDto>>(seats) ?? new List<SeatDto>();

            return seatsDto;
        }

        /// <summary>
        /// Re-creates events when area was changed.
        /// </summary>
        /// <param name="areaId">Area id.</param>
        /// <returns>Task.</returns>
        public async Task RecreateEvents(int areaId)
        {
            var area = await _areaRepository.GetByIdAsync(areaId);
            var events = _eventRepository.GetAll().Where(e => e.LayoutId == area.LayoutId).ToList();
            foreach (var eventItem in events)
            {
                var areas = _eventAreaRepository.GetAll().Where(a => eventItem.Id == a.EventId).ToList();
                var seats = _eventSeatRepository.GetAll().AsEnumerable().Where(s => areas.Any(a => a.Id == s.EventAreaId));
                if (seats.All(s => s.State == 0))
                {
                    await _eventRepository.DeleteAsync(eventItem);
                    await _eventRepository.CreateAsync(eventItem);
                }
            }
        }
    }
}
