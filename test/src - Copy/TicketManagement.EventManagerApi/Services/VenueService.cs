using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.EventManagerApi.Models;
using TicketManagement.EventManagerApi.Services.Interfaces;

namespace TicketManagement.EventManagerApi.Services
{
    /// <summary>
    /// Responsible for proxy venue repository calls.
    /// </summary>
    internal class VenueService : IVenueService
    {
        private readonly IRepository<Venue> _venueRepository;
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="VenueService"/> class.
        /// </summary>
        /// <param name="venueRepository">Venue repository.</param>
        /// <param name="layoutRepository">Layout repository.</param>
        /// <param name="mapper">Mapper.</param>
        public VenueService(IRepository<Venue> venueRepository, IRepository<Layout> layoutRepository, IMapper mapper)
        {
            _venueRepository = venueRepository;
            _layoutRepository = layoutRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public List<VenueModel> GetVenues()
        {
            var venues = _venueRepository.GetAll();
            var layouts = _layoutRepository.GetAll();
            var venueModels = _mapper.Map<IQueryable<Venue>, List<VenueModel>>(venues);
            var layoutModels = _mapper.Map<IQueryable<Layout>, List<LayoutModel>>(layouts);
            venueModels.ForEach(v => v.Layouts.AddRange(layoutModels.Where(l => l.VenueId == v.Id)));
            return venueModels;
        }

        /// <inheritdoc />
        public List<VenueModel> GetVenuesBy(Expression<Func<Venue, bool>> predicate)
        {
            var venues = _venueRepository.GetAll().Where(predicate);
            var venuesDto = _mapper.Map<IQueryable<Venue>, List<VenueModel>>(venues) ?? new List<VenueModel>();

            return venuesDto;
        }
    }
}
