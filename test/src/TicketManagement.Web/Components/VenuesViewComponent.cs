using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.Web.Models;

namespace TicketManagement.Web.Components
{
    /// <summary>
    /// View component for retreiving venues with related data.
    /// </summary>
    public class VenuesViewComponent : ViewComponent
    {
        private readonly IVenueService _venueService;
        private readonly ILayoutService _layoutService;
        private readonly IAreaService _areaService;
        private readonly ISeatService _seatService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VenuesViewComponent"/> class.
        /// </summary>
        /// <param name="venueService">Venue service.</param>
        /// <param name="layoutService">Layout service.</param>
        /// <param name="areaService">Area service.</param>
        /// <param name="seatService">Seat service.</param>
        public VenuesViewComponent(IVenueService venueService,
            ILayoutService layoutService, IAreaService areaService, ISeatService seatService)
        {
            _venueService = venueService;
            _layoutService = layoutService;
            _areaService = areaService;
            _seatService = seatService;
        }

        /// <summary>
        /// Gets venues with related data.
        /// </summary>
        /// <returns>View component with venue view models.</returns>
        public IViewComponentResult Invoke()
        {
            var venues = _venueService.GetVenues().ToList();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<VenueDto, VenueViewModel>()).CreateMapper();
            var venueModels = mapper.Map<List<VenueDto>, List<VenueViewModel>>(venues);

            mapper = new MapperConfiguration(cfg => cfg.CreateMap<LayoutDto, LayoutViewModel>()).CreateMapper();
            venueModels.ForEach(v => v.Layouts = mapper.Map<List<LayoutDto>, List<LayoutViewModel>>(_layoutService.GetLayouts().Where(l => l.VenueId == v.Id).ToList()));

            mapper = new MapperConfiguration(cfg => cfg.CreateMap<AreaDto, AreaViewModel>()).CreateMapper();
            venueModels.ForEach(v => v.Layouts.ForEach(l => l.Areas =
                mapper.Map<List<AreaDto>, List<AreaViewModel>>(_areaService.GetAreas()
                .Where(a => a.LayoutId == l.Id).ToList())));

            mapper = new MapperConfiguration(cfg => cfg.CreateMap<SeatDto, SeatViewModel>()).CreateMapper();
            venueModels.ForEach(v => v.Layouts.ForEach(l => l.Areas.ForEach(a => a.Seats =
                mapper.Map<List<SeatDto>, List<SeatViewModel>>(_seatService.GetSeats()
                .Where(seat => seat.AreaId == a.Id).ToList()))));

            return View(venueModels);
        }
    }
}
