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
    /// Responsible for proxy event seat repository calls.
    /// </summary>
    public class EventSeatService : IEventSeatService
    {
        private readonly IRepository<EventSeat> _eventSeatRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSeatService"/> class.
        /// </summary>
        /// <param name="eventSeatRepository">Event seat repository.</param>
        /// <param name="mapper">Mapper.</param>
        public EventSeatService(IRepository<EventSeat> eventSeatRepository, IMapper mapper)
        {
            _eventSeatRepository = eventSeatRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task UpdateEventSeatAsync(EventSeatModel eventSeat)
        {
            var seatEntity = _mapper.Map<EventSeatModel, EventSeat>(eventSeat);
            await _eventSeatRepository.UpdateAsync(seatEntity);
        }

        /// <inheritdoc />
        public async Task<EventSeatModel> GetEventSeatAsync(int eventSeatId)
        {
            var eventSeat = await _eventSeatRepository.GetByIdAsync(eventSeatId);
            var eventSeatModel = _mapper
                .Map<EventSeat, EventSeatModel>(eventSeat);

            return eventSeatModel;
        }

        /// <inheritdoc />
        public List<EventSeatModel> GetEventSeats()
        {
            var eventSeats = _eventSeatRepository.GetAll();
            List<EventSeatModel> eventSeatModels = _mapper
                .Map<IQueryable<EventSeat>, List<EventSeatModel>>(eventSeats) ?? new List<EventSeatModel>();

            return eventSeatModels;
        }

        /// <inheritdoc />
        public List<EventSeatModel> GetEventSeatsBy(Expression<Func<EventSeat, bool>> predicate)
        {
            var eventSeats = _eventSeatRepository.GetAll().Where(predicate);
            List<EventSeatModel> eventSeatModels = _mapper
                .Map<IQueryable<EventSeat>, List<EventSeatModel>>(eventSeats) ?? new List<EventSeatModel>();

            return eventSeatModels;
        }
    }
}
