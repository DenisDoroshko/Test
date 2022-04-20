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
    /// Responsible for proxy all area repository calls with validation logic.
    /// </summary>
    internal class AreaService : BaseService<AreaDto>, IAreaService
    {
        private readonly IRepository<Area> _areaRepository;

        private readonly IRepository<Seat> _seatRepository;

        private readonly IRepository<Event> _eventRepository;

        private readonly IRepository<EventArea> _eventAreaRepository;

        private readonly IRepository<EventSeat> _eventSeatRepository;

        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaService"/> class.
        /// </summary>
        /// <param name="layoutRepository">Layout repository.</param>
        /// <param name="areaRepository">Area repository.</param>
        /// <param name="seatRepository">Seat repository.</param>
        /// <param name="eventRepository">Event repository.</param>
        /// <param name="eventAreaRepository">Event area repository.</param>
        /// <param name="eventSeatRepository">Event seat repository.</param>
        /// <param name="mapper">Mapper.</param>
        public AreaService(IRepository<Layout> layoutRepository, IRepository<Area> areaRepository,
            IRepository<Seat> seatRepository, IRepository<Event> eventRepository, IRepository<EventArea> eventAreaRepository,
            IRepository<EventSeat> eventSeatRepository, IMapper mapper)
        {
            _areaRepository = areaRepository;
            _seatRepository = seatRepository;
            _eventRepository = eventRepository;
            _eventAreaRepository = eventAreaRepository;
            _eventSeatRepository = eventSeatRepository;
            _mapper = mapper;
            Validator = new AreaValidator(layoutRepository, areaRepository, seatRepository);
        }

        /// <summary>
        /// Gets or sets validator for area.
        /// </summary>
        protected override AbstractValidator<AreaDto> Validator { get; set; }

        /// <summary>
        /// Creates area with validation.
        /// </summary>
        /// <param name="areaDto">Area dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> CreateAreaAsync(AreaDto areaDto)
        {
            var result = await TryValidateAsync(areaDto, "Create");
            if (result.IsValid)
            {
                await RecreateEvents(areaDto.LayoutId);

                var area = _mapper.Map<AreaDto, Area>(areaDto);
                areaDto.Id = await _areaRepository.CreateAsync(area);
            }

            return result;
        }

        /// <summary>
        /// Updates area with validation.
        /// </summary>
        /// <param name="areaDto">Area dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> UpdateAreaAsync(AreaDto areaDto)
        {
            var result = await TryValidateAsync(areaDto, "Update");
            if (result.IsValid)
            {
                await RecreateEvents(areaDto.LayoutId);

                var area = _mapper.Map<AreaDto, Area>(areaDto);
                await _areaRepository.UpdateAsync(area);
            }

            return result;
        }

        /// <summary>
        /// Deletes area with validation.
        /// </summary>
        /// <param name="areaDto">Area dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> DeleteAreaAsync(AreaDto areaDto)
        {
            var result = await TryValidateAsync(areaDto, "Delete");
            if (result.IsValid)
            {
                await RecreateEvents(areaDto.LayoutId);

                var areaSeats = _seatRepository.GetAll().Where(s => s.AreaId == areaDto.Id).ToList();
                for (var i = 0; i < areaSeats.Count; i++)
                {
                    await _seatRepository.DeleteAsync(areaSeats[i]);
                }

                var area = _mapper.Map<AreaDto, Area>(areaDto);
                await _areaRepository.DeleteAsync(area);
            }

            return result;
        }

        /// <summary>
        /// Gets area from data storage by specified id.
        /// </summary>
        /// <param name="areaId">Specified id of the area.</param>
        /// <returns>Area dto if the operation was successful otherwise null.</returns>
        public async Task<AreaDto> GetAreaAsync(int areaId)
        {
            AreaDto areaDto = null;
            var area = await _areaRepository.GetByIdAsync(areaId);
            if (area != null)
            {
                areaDto = _mapper.Map<Area, AreaDto>(area);
            }

            return areaDto;
        }

        /// <summary>
        /// Gets all area from data storage.
        /// </summary>
        /// <returns>Area list if the operation was successful otherwise empty area list.</returns>
        public List<AreaDto> GetAreas()
        {
            var areas = _areaRepository.GetAll();
            var areasDto = _mapper.Map<IQueryable<Area>, List<AreaDto>>(areas) ?? new List<AreaDto>();

            return areasDto;
        }

        /// <summary>
        /// Gets specified areas.
        /// </summary>
        /// <returns>Specified Area list if the operation was successful otherwise empty areas list.</returns>
        public List<AreaDto> GetAreasBy(Expression<Func<Area, bool>> predicate)
        {
            var areas = _areaRepository.GetAll().Where(predicate);
            var areasDto = _mapper.Map<IQueryable<Area>, List<AreaDto>>(areas) ?? new List<AreaDto>();

            return areasDto;
        }

        /// <summary>
        /// Re-creates events when area was changed.
        /// </summary>
        /// <param name="layoutId">Layout id.</param>
        /// <returns>Task.</returns>
        private async Task RecreateEvents(int layoutId)
        {
            var events = _eventRepository.GetAll().Where(e => e.LayoutId == layoutId).ToList();
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
