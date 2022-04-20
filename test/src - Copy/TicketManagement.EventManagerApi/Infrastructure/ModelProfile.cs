using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TicketManagement.DataAccess.Entities;
using TicketManagement.EventManagerApi.Models;

namespace TicketManagement.EventManagerApi.Infrastructure
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<VenueModel, Venue>();
            CreateMap<Venue, VenueModel>();
            CreateMap<LayoutModel, Layout>();
            CreateMap<Layout, LayoutModel>();
            CreateMap<EventModel, Event>();
            CreateMap<Event, EventModel>();
            CreateMap<EventAreaModel, EventArea>();
            CreateMap<EventArea, EventAreaModel>();
            CreateMap<EventSeatModel, EventSeat>();
            CreateMap<EventSeat, EventSeatModel>();
            CreateMap<AreaModel, Area>();
            CreateMap<Area, AreaModel>();
            CreateMap<SeatModel, Seat>();
            CreateMap<Seat, SeatModel>();
            CreateMap<Layout, LayoutModel>();
            CreateMap<LayoutModel, Layout>();
        }
    }
}
