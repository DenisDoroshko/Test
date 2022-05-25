using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Validators
{
    /// <summary>
    /// Validator for the venue with different rule sets.
    /// </summary>
    public class VenueValidator : AbstractValidator<VenueDto>
    {
        private readonly IRepository<Venue> _venueRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="VenueValidator"/> class.
        /// </summary>
        /// <param name="venueRepository">Venue repository.</param>
        public VenueValidator(IRepository<Venue> venueRepository)
        {
            _venueRepository = venueRepository;
            RuleSet("Create", () =>
            {
                RuleFor(v => v)
                    .Must(v => IsUniqueNameDuringCreate(v))
                    .WithMessage("Can not create venue with an already existing name.");
            });

            RuleSet("Update", () =>
            {
                RuleFor(v => v.Id)
                    .MustAsync(async (venueId, cancancellation) => await IsExistVenueAsync(venueId))
                    .WithMessage("There is no venue with this id.");
                RuleFor(v => v)
                    .Must(v => IsUniqueNameDuringUpdate(v))
                    .WithMessage("Can not update venue with an already existing name.");
            });

            RuleSet("Delete", () =>
            {
                RuleFor(v => v.Id)
                    .MustAsync(async (venueId, cancancellation) => await IsExistVenueAsync(venueId))
                    .WithMessage("There is no venue with this id.");
            });
        }

        /// <summary>
        /// Checks if the venue name unique(for create rule).
        /// </summary>
        /// <param name="venue">Venue.</param>
        /// <returns>True if unique otherwise false.</returns>
        private bool IsUniqueNameDuringCreate(VenueDto venue)
        {
            return _venueRepository.GetAll().All(item => item.Name != venue.Name);
        }

        /// <summary>
        /// Checks if the venue name unique(for update rule).
        /// Allows the name to match for venues with the same id.
        /// </summary>
        /// <param name="venue">Venue.</param>
        /// <returns>True if unique otherwise false.</returns>
        private bool IsUniqueNameDuringUpdate(VenueDto venue)
        {
            return _venueRepository.GetAll().All(item => item.Name != venue.Name || item.Id == venue.Id);
        }

        /// <summary>
        /// Checks if the venue  exists.
        /// </summary>
        /// <param name="venueId">Venue id.</param>
        /// <returns>True if exists otherwise false.</returns>
        private async Task<bool> IsExistVenueAsync(int venueId)
        {
            return await _venueRepository.GetByIdAsync(venueId) != null;
        }
    }
}
