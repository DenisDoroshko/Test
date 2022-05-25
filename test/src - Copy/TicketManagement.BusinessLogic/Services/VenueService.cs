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
    /// Responsible for proxy all venue repository calls with validation logic.
    /// </summary>
    internal class VenueService : BaseService<VenueDto>, IVenueService
    {
        private readonly IRepository<Venue> _venueRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="VenueService"/> class.
        /// </summary>
        /// <param name="venueRepository">Venue repository.</param>
        /// <param name="mapper">Mapper.</param>
        public VenueService(IRepository<Venue> venueRepository, IMapper mapper)
        {
            _venueRepository = venueRepository;
            _mapper = mapper;
            Validator = new VenueValidator(venueRepository);
        }

        /// <summary>
        /// Gets or sets validator for venue.
        /// </summary>
        protected override AbstractValidator<VenueDto> Validator { get; set; }

        /// <summary>
        /// Creates venue with validation.
        /// </summary>
        /// <param name="venueDto">Venue dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> CreateVenueAsync(VenueDto venueDto)
        {
            var result = await TryValidateAsync(venueDto, "Create");
            if (result.IsValid)
            {
                var venue = _mapper.Map<VenueDto, Venue>(venueDto);
                venueDto.Id = await _venueRepository.CreateAsync(venue);
            }

            return result;
        }

        /// <summary>
        /// Updates venue with validation.
        /// </summary>
        /// <param name="venueDto">Venue dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> UpdateVenueAsync(VenueDto venueDto)
        {
            var result = await TryValidateAsync(venueDto, "Update");
            if (result.IsValid)
            {
                var venue = _mapper.Map<VenueDto, Venue>(venueDto);
                await _venueRepository.UpdateAsync(venue);
            }

            return result;
        }

        /// <summary>
        /// Deletes venue with validation.
        /// </summary>
        /// <param name="venueDto">Venue dto.</param>
        /// <returns>Validation result.</returns>
        public async Task<ValidationResult> DeleteVenueAsync(VenueDto venueDto)
        {
            var result = await TryValidateAsync(venueDto, "Delete");
            if (result.IsValid)
            {
                var venue = _mapper.Map<VenueDto, Venue>(venueDto);
                await _venueRepository.DeleteAsync(venue);
            }

            return result;
        }

        /// <summary>
        /// Gets venue from data storage by specified id.
        /// </summary>
        /// <param name="venueId">Specified id of the venue.</param>
        /// <returns>Venue dto if the operation was successful otherwise null.</returns>
        public async Task<VenueDto> GetVenueAsync(int venueId)
        {
            VenueDto venueDto = null;
            var venue = await _venueRepository.GetByIdAsync(venueId);
            if (venue != null)
            {
                venueDto = _mapper.Map<Venue, VenueDto>(venue);
            }

            return venueDto;
        }

        /// <summary>
        /// Gets all venues from data storage.
        /// </summary>
        /// <returns>Venue list if the operation was successful otherwise empty venue list.</returns>
        public List<VenueDto> GetVenues()
        {
            var venues = _venueRepository.GetAll();
            var venuesDto = _mapper.Map<IQueryable<Venue>, List<VenueDto>>(venues) ?? new List<VenueDto>();

            return venuesDto;
        }

        /// <summary>
        /// Gets specified venues.
        /// </summary>
        /// <returns>Specified Venue list if the operation was successful otherwise empty venue list.</returns>
        public List<VenueDto> GetVenuesBy(Expression<Func<Venue, bool>> predicate)
        {
            var venues = _venueRepository.GetAll().Where(predicate);
            var venuesDto = _mapper.Map<IQueryable<Venue>, List<VenueDto>>(venues) ?? new List<VenueDto>();

            return venuesDto;
        }
    }
}
