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
    /// Responsible for proxy event area repository calls.
    /// </summary>
    public class EventAreaService : IEventAreaService
    {
        private readonly IRepository<EventArea> _eventAreaRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAreaService"/> class.
        /// </summary>
        /// <param name="eventAreaRepository">Event area repository.</param>
        /// <param name="mapper">Mapper.</param>
        public EventAreaService(IRepository<EventArea> eventAreaRepository, IMapper mapper)
        {
            _eventAreaRepository = eventAreaRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task UpdateEventAreaAsync(EventAreaModel eventAreaModel)
        {
            var areaDto = _mapper.Map<EventAreaModel, EventArea>(eventAreaModel);
            await _eventAreaRepository.UpdateAsync(areaDto);
        }

        /// <inheritdoc />
        public async Task<EventAreaModel> GetEventAreaAsync(int eventAreaId)
        {
            var eventArea = await _eventAreaRepository.GetByIdAsync(eventAreaId);
            var eventAreaModel = _mapper
                .Map<EventArea, EventAreaModel>(eventArea);

            return eventAreaModel;
        }

        /// <inheritdoc />
        public List<EventAreaModel> GetEventAreas()
        {
            var eventAreas = _eventAreaRepository.GetAll();
            var eventAreaModels = _mapper
                .Map<IQueryable<EventArea>, List<EventAreaModel>>(eventAreas) ?? new List<EventAreaModel>();

            return eventAreaModels;
        }

        /// <inheritdoc />
        public List<EventAreaModel> GetEventAreasBy(Expression<Func<EventArea, bool>> predicate)
        {
            var eventAreas = _eventAreaRepository.GetAll().Where(predicate);
            var eventAreaModels = _mapper
                .Map<IQueryable<EventArea>, List<EventAreaModel>>(eventAreas) ?? new List<EventAreaModel>();

            return eventAreaModels;
        }
    }
}
