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
    /// Validator for the area with different rule sets.
    /// </summary>
    public class AreaValidator : AbstractValidator<AreaDto>
    {
        private readonly IRepository<Layout> _layoutRepository;

        private readonly IRepository<Area> _areaRepository;

        private readonly IRepository<Seat> _seatRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaValidator"/> class.
        /// </summary>
        /// <param name="layoutRepository">Layout repository.</param>
        /// <param name="areaRepository">Area repository.</param>
        /// <param name="seatRepository">Seat repository.</param>
        public AreaValidator(IRepository<Layout> layoutRepository, IRepository<Area> areaRepository, IRepository<Seat> seatRepository)
        {
            _layoutRepository = layoutRepository;
            _areaRepository = areaRepository;
            _seatRepository = seatRepository;
            RuleSet("Create", () =>
            {
                RuleFor(a => a.LayoutId)
                    .MustAsync(async (layoutId, cancellation) => await IsExistLayoutForAreaAsync(layoutId))
                    .WithMessage("There is no layout for this area.");
                RuleFor(a => a)
                    .Must(a => IsUniqueDescriptionDuringCreate(a))
                    .WithMessage("Area description should be unique in layout.");
                RuleFor(a => a)
                    .MustAsync(async (area, cancellation) => await IsCorrectSizeAsync(area))
                    .WithMessage("Area size can't be more than layout size.");
            });

            RuleSet("Update", () =>
            {
                RuleFor(a => a.Id)
                    .MustAsync(async (areaId, cancellation) => await IsExistAreaAsync(areaId))
                    .WithMessage("There is no area with this id.");
                RuleFor(a => a)
                    .Must(a => IsUniqueDescriptionDuringUpdate(a))
                    .WithMessage("Area description should be unique in layout.");
                RuleFor(a => a)
                    .MustAsync(async (area, cancellation) => await IsCorrectLocation(area))
                    .WithMessage("Area location is incorrect.");
                RuleFor(a => a)
                    .MustAsync(async (area, cancellation) => await IsCorrectSizeAsync(area))
                    .WithMessage("Area size can't be more than layout size.");
            });

            RuleSet("Delete", () =>
            {
                RuleFor(a => a.Id)
                    .MustAsync(async (areaId, cancellation) => await IsExistAreaAsync(areaId))
                    .WithMessage("There is no area with this id.");
            });
        }

        /// <summary>
        /// Checks if the layout for area exists.
        /// </summary>
        /// <param name="layoutId">Layout id.</param>
        /// <returns>True if layout for area exists otherwise false.</returns>
        private async Task<bool> IsExistLayoutForAreaAsync(int layoutId)
        {
            return await _layoutRepository.GetByIdAsync(layoutId) != null;
        }

        /// <summary>
        /// Checks if the area description unique(for create rule).
        /// </summary>
        /// <param name="area">Area.</param>
        /// <returns>True if unique otherwise false.</returns>
        private bool IsUniqueDescriptionDuringCreate(AreaDto area)
        {
            var loadedAreas = _areaRepository.GetAll().Where(item => item.LayoutId == area.LayoutId);
            return loadedAreas.All(item => item.Description != area.Description);
        }

        /// <summary>
        /// Checks if the area description unique(for update rule).
        /// Allows the description to match for areas with the same id.
        /// </summary>
        /// <param name="area">Area.</param>
        /// <returns>True if unique otherwise false.</returns>
        private bool IsUniqueDescriptionDuringUpdate(AreaDto area)
        {
            var loadedAreas = _areaRepository.GetAll().Where(item => item.LayoutId == area.LayoutId);
            return loadedAreas.All(item => item.Description != area.Description || item.Id == area.Id);
        }

        /// <summary>
        /// Checks if the area  exists.
        /// </summary>
        /// <param name="areaId">Area id.</param>
        /// <returns>True if exists otherwise false.</returns>
        private async Task<bool> IsExistAreaAsync(int areaId)
        {
            return (await _areaRepository.GetByIdAsync(areaId)) != null;
        }

        /// <summary>
        /// Checks if the area  exists.
        /// </summary>
        /// <param name="area">Area.</param>
        /// <returns>True if area size is correct otherwise false.</returns>
        private async Task<bool> IsCorrectSizeAsync(AreaDto area)
        {
            var layout = await _layoutRepository.GetByIdAsync(area.LayoutId);
            return layout != null && layout.Width >= area.Width && layout.Height >= area.Height;
        }

        /// <summary>
        /// Checks if the area location is correct.
        /// </summary>
        /// <param name="area">Area id.</param>
        /// <returns>True if is correct otherwise false.</returns>
        private async Task<bool> IsCorrectLocation(AreaDto area)
        {
            if (area.CoordX != 0 && area.CoordY != 0)
            {
                var layout = await _layoutRepository.GetByIdAsync(area.LayoutId);
                var areaSeats = _seatRepository.GetAll().Where(s => s.AreaId == area.Id)
                    .Select(s => new { Number = s.Number + area.CoordX - 1, Row = s.Row + area.CoordY - 1 });
                var layoutAreas = _areaRepository.GetAll()
                    .Where(a => a.LayoutId == area.LayoutId && a.Id != area.Id);
                var layoutSeats = _seatRepository.GetAll()
                    .Where(s => layoutAreas.Any(a => a.Id == s.AreaId))
                    .Join(layoutAreas, s => s.AreaId, a => a.Id, (s, a) =>
                    new { Number = s.Number + a.CoordX - 1, Row = s.Row + a.CoordY - 1, });

                return areaSeats.All(s => s.Number <= layout.Width && s.Row <= layout.Height)
                    && areaSeats.All(areaSeat => layoutSeats
                    .All(layoutSeat => layoutSeat.Number != areaSeat.Number || layoutSeat.Row != areaSeat.Row));
            }
            else
            {
                return true;
            }
        }
    }
}
