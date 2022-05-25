using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validators
{
    /// <summary>
    /// Validator for the layout with different rule sets.
    /// </summary>
    public class LayoutValidator : AbstractValidator<LayoutDto>
    {
        private readonly IRepository<Venue> _venueRepository;

        private readonly IRepository<Layout> _layoutRepository;

        private readonly IRepository<Area> _areaRepository;

        private readonly IRepository<Seat> _seatRepository;

        private readonly IRepository<Event> _eventRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutValidator"/> class.
        /// </summary>
        /// <param name="venueRepository">Venue repository.</param>
        /// <param name="layoutRepository">Layout repository.</param>
        /// <param name="areaRepository">Area repository.</param>
        /// <param name="seatRepository">Seat repository.</param>
        /// <param name="eventRepository">Event repository.</param>
        public LayoutValidator(IRepository<Venue> venueRepository, IRepository<Layout> layoutRepository,
            IRepository<Area> areaRepository, IRepository<Seat> seatRepository, IRepository<Event> eventRepository)
        {
            _venueRepository = venueRepository;
            _layoutRepository = layoutRepository;
            _areaRepository = areaRepository;
            _seatRepository = seatRepository;
            _eventRepository = eventRepository;
            RuleSet("Create", () =>
            {
                RuleFor(l => l.VenueId)
                    .MustAsync(async (venueId, cancancellation) => await IsExistVenueForLayoutAsync(venueId))
                    .WithMessage("There is no venue for this layout.");
                RuleFor(l => l)
                    .Must(l => IsUniqueNameDuringCreate(l))
                    .WithMessage("Can not create layout with an already existing name in venue.");
            });

            RuleSet("Update", () =>
            {
                RuleFor(l => l.Id)
                    .MustAsync(async (layoutId, cancancellation) => await IsExistLayoutAsync(layoutId))
                    .WithMessage("There is no layout with this id.");
                RuleFor(l => l)
                    .Must(l => IsUniqueNameDuringUpdate(l))
                    .WithMessage("Can not update layout with an already existing name in venue.");
                RuleFor(l => l)
                    .Must(l => IsCorrectSize(l))
                    .WithMessage("Layout size is too small.");
            });

            RuleSet("Delete", () =>
            {
                RuleFor(l => l.Id)
                    .MustAsync(async (layoutId, cancancellation) => await IsExistLayoutAsync(layoutId))
                    .WithMessage("There is no layout with this id.");
                RuleFor(l => l.Id)
                    .Must(layoutId => IsExistEvents(layoutId))
                    .WithMessage("Layout contain related events.");
            });
        }

        /// <summary>
        /// Checks if the venue for layout exists.
        /// </summary>
        /// <param name="venueId">Venue id.</param>
        /// <returns>True if venue for layout exists otherwise false.</returns>
        private async Task<bool> IsExistVenueForLayoutAsync(int venueId)
        {
            return await _venueRepository.GetByIdAsync(venueId) != null;
        }

        /// <summary>
        /// Checks if the layout name unique(for create rule).
        /// </summary>
        /// <param name="layout">Layout.</param>
        /// <returns>True if unique otherwise false.</returns>
        private bool IsUniqueNameDuringCreate(LayoutDto layout)
        {
            var loadedLayouts = _layoutRepository.GetAll().Where(item => item.VenueId == layout.VenueId);
            return loadedLayouts.All(item => item.Name != layout.Name);
        }

        /// <summary>
        /// Checks if the layout name unique(for update rule).
        /// Allows the name to match for layouts with the same id.
        /// </summary>
        /// <param name="layout">Layout.</param>
        /// <returns>True if unique otherwise false.</returns>
        private bool IsUniqueNameDuringUpdate(LayoutDto layout)
        {
            var loadedLayouts = _layoutRepository.GetAll().Where(item => item.VenueId == layout.VenueId);
            return loadedLayouts.All(item => item.Name != layout.Name || item.Id == layout.Id);
        }

        /// <summary>
        /// Checks if the layout  exists.
        /// </summary>
        /// <param name="layoutId">Layout id.</param>
        /// <returns>True if exists otherwise false.</returns>
        private async Task<bool> IsExistLayoutAsync(int layoutId)
        {
            return await _layoutRepository.GetByIdAsync(layoutId) != null;
        }

        /// <summary>
        /// Checks if the layout  size is correct.
        /// </summary>
        /// <param name="layout">Layout dto.</param>
        /// <returns>True if size is correct otherwise false.</returns>
        private bool IsCorrectSize(LayoutDto layout)
        {
            var layoutAreas = _areaRepository.GetAll().Where(a => a.LayoutId == layout.Id);
            var layoutSeats = _seatRepository.GetAll()
                .Where(s => layoutAreas.Any(a => a.Id == s.AreaId))
                .Join(layoutAreas, s => s.AreaId, a => a.Id, (s, a) =>
                new { Number = s.Number + a.CoordX - 1, Row = s.Row + a.CoordY - 1, });
            return layoutSeats.All(s => s.Row <= layout.Height && s.Number <= layout.Width);
        }

        /// <summary>
        /// Checks if the layout  size is correct.
        /// </summary>
        /// <param name="layoutId">Layout id.</param>
        /// <returns>True if there are no related events otherwise false.</returns>
        private bool IsExistEvents(int layoutId)
        {
            return !_eventRepository.GetAll().Where(e => e.LayoutId == layoutId).Any();
        }
    }
}
