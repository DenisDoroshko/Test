using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.DataAccess.Entities;
using TicketManagement.Web.Models;

namespace TicketManagement.Web.Infrastructure.MapperProfile
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<VenueViewModel, VenueDto>();
            CreateMap<VenueDto, VenueViewModel>();
            CreateMap<LayoutViewModel, LayoutDto>();
            CreateMap<LayoutDto, LayoutViewModel>();
            CreateMap<AreaViewModel, AreaDto>();
            CreateMap<AreaDto, AreaViewModel>();
            CreateMap<SeatDto, SeatViewModel>();
            CreateMap<SeatViewModel, SeatDto>();
            CreateMap<EventViewModel, EventDto>();
            CreateMap<EventDto, EventViewModel>();
            CreateMap<EventAreaViewModel, EventAreaDto>();
            CreateMap<EventAreaDto, EventAreaViewModel>();
            CreateMap<EventSeatViewModel, EventSeatDto>();
            CreateMap<EventSeatDto, EventSeatViewModel>();
            CreateMap<Venue, VenueDto>();
            CreateMap<VenueDto, Venue>();
            CreateMap<Layout, LayoutDto>();
            CreateMap<LayoutDto, Layout>();
            CreateMap<Area, AreaDto>();
            CreateMap<AreaDto, Area>();
            CreateMap<SeatDto, Seat>();
            CreateMap<Seat, SeatDto>();
            CreateMap<Event, EventDto>();
            CreateMap<EventDto, Event>();
            CreateMap<EventArea, EventAreaDto>();
            CreateMap<EventAreaDto, EventArea>();
            CreateMap<EventSeat, EventSeatDto>();
            CreateMap<EventSeatDto, EventSeat>();
        }
    }
}
