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
    /// Responsible for proxy all layout repository calls with validation logic.
    /// </summary>
    internal class LayoutService : BaseService<LayoutDto>, ILayoutService
    {
        private readonly IRepository<Layout> _layoutRepository;

        private readonly IRepository<Area> _areaRepository;

        private readonly IRepository<Seat> _seatRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutService"/> class.
        /// </summary>
        /// <param name="venueRepository">Venue repository.</param>
        /// <param name="layoutRepository">Layout repository.</param>
        /// <param name="areaRepository">Area repository.</param>
        /// <param name="seatRepository">Seat repository.</param>
        /// <param name="eventRepository">Event repository.</param>
        /// <param name="mapper">Mapper.</param>
        public LayoutService(IRepository<Venue> venueRepository, IRepository<Layout> layoutRepository,
            IRepository<Area> areaRepository, IRepository<Seat> seatRepository,
            IRepository<Event> eventRepository, IMapper mapper)
        {
            _layoutRepository = layoutRepository;
            _areaRepository = areaRepository;
            _seatRepository = seatRepository;
            _mapper = mapper;
            Validator = new LayoutValidator(venueRepository, layoutRepository,
                areaRepository, seatRepository, eventRepository);
        }

        /// <summary>
        /// Gets or sets validator for layout.
        /// </summary>
        protected override AbstractValidator<LayoutDto> Validator { get; set; }

        /// <summary>
        /// Creates layout with validation.
        /// </summary>
        /// <param name="layoutDto">Layout dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> CreateLayoutAsync(LayoutDto layoutDto)
        {
            var result = await TryValidateAsync(layoutDto, "Create");
            if (result.IsValid)
            {
                var layout = _mapper.Map<LayoutDto, Layout>(layoutDto);
                layoutDto.Id = await _layoutRepository.CreateAsync(layout);
            }

            return result;
        }

        /// <summary>
        /// Updates layout with validation.
        /// </summary>
        /// <param name="layoutDto">Layout dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> UpdateLayoutAsync(LayoutDto layoutDto)
        {
            var result = await TryValidateAsync(layoutDto, "Update");
            if (result.IsValid)
            {
                var layout = _mapper.Map<LayoutDto, Layout>(layoutDto);
                await _layoutRepository.UpdateAsync(layout);
            }

            return result;
        }

        /// <summary>
        /// Deletes layout with validation.
        /// </summary>
        /// <param name="layoutDto">Layout dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> DeleteLayoutAsync(LayoutDto layoutDto)
        {
            var result = await TryValidateAsync(layoutDto, "Delete");
            if (result.IsValid)
            {
                var layoutAreas = _areaRepository.GetAll().Where(a => a.LayoutId == layoutDto.Id).ToList();
                var layoutSeats = _seatRepository.GetAll().AsEnumerable()
                    .Where(s => layoutAreas.Any(a => a.Id == s.AreaId)).ToList();
                for (var i = 0; i < layoutSeats.Count; i++)
                {
                    await _seatRepository.DeleteAsync(layoutSeats[i]);
                }

                for (var i = 0; i < layoutAreas.Count; i++)
                {
                    await _areaRepository.DeleteAsync(layoutAreas[i]);
                }

                var layout = _mapper.Map<LayoutDto, Layout>(layoutDto);
                await _layoutRepository.DeleteAsync(layout);
            }

            return result;
        }

        /// <summary>
        /// Gets layout from data storage by specified id.
        /// </summary>
        /// <param name="layoutId">Specified id of the layout.</param>
        /// <returns>Layout dto if the operation was successful otherwise null.</returns>
        public async Task<LayoutDto> GetLayoutAsync(int layoutId)
        {
            LayoutDto layoutDto = null;
            var layout = await _layoutRepository.GetByIdAsync(layoutId);
            if (layout != null)
            {
                layoutDto = _mapper.Map<Layout, LayoutDto>(layout);
            }

            return layoutDto;
        }

        /// <summary>
        /// Gets all layouts from data storage.
        /// </summary>
        /// <returns>Layout list if the operation was successful otherwise empty layout list.</returns>
        public List<LayoutDto> GetLayouts()
        {
            var layouts = _layoutRepository.GetAll();
            var layoutsDto = _mapper
                .Map<IQueryable<Layout>, List<LayoutDto>>(layouts) ?? new List<LayoutDto>();

            return layoutsDto;
        }

        /// <summary>
        /// Gets specified layouts.
        /// </summary>
        /// <returns>Specified Layout list if the operation was successful otherwise empty layout list.</returns>
        public List<LayoutDto> GetLayoutsBy(Expression<Func<Layout, bool>> predicate)
        {
            var layouts = _layoutRepository.GetAll().Where(predicate);
            var layoutsDto = _mapper
                .Map<IQueryable<Layout>, List<LayoutDto>>(layouts) ?? new List<LayoutDto>();

            return layoutsDto;
        }
    }
}
