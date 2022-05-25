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
    /// Responsible for proxy layout repository calls.
    /// </summary>
    internal class LayoutService : ILayoutService
    {
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IRepository<Area> _areaRepository;
        private readonly IRepository<Seat> _seatRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutService"/> class.
        /// </summary>
        /// <param name="layoutRepository">Layout repository.</param>
        /// <param name="areaRepository">Area repository.</param>
        /// <param name="seatRepository">Seat repository.</param>
        /// <param name="mapper">Mapper for layout.</param>
        public LayoutService(IRepository<Layout> layoutRepository, IRepository<Area> areaRepository, IRepository<Seat> seatRepository, IMapper mapper)
        {
            _layoutRepository = layoutRepository;
            _areaRepository = areaRepository;
            _seatRepository = seatRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<LayoutModel> GetLayoutAsync(int layoutModelId)
        {
            var layout = await _layoutRepository.GetByIdAsync(layoutModelId);
            var layoutModel = _mapper.Map<Layout, LayoutModel>(layout);
            var areas = _areaRepository.GetAll().Where(a => a.LayoutId == layoutModel.Id);
            var areaModels = _mapper.Map<IQueryable<Area>, List<AreaModel>>(areas);
            foreach (var area in areaModels)
            {
                var areaSeats = _seatRepository.GetAll().Where(s => s.AreaId == area.Id);
                var seatModels = _mapper.Map<IQueryable<Seat>, List<SeatModel>>(areaSeats);
                area.Seats = seatModels;
            }

            layoutModel.Areas = areaModels;
            return layoutModel;
        }

        /// <inheritdoc />
        public List<LayoutModel> GetLayoutsBy(Expression<Func<Layout, bool>> predicate)
        {
            var layouts = _layoutRepository.GetAll().Where(predicate);
            var layoutsDto = _mapper
                .Map<IQueryable<Layout>, List<LayoutModel>>(layouts) ?? new List<LayoutModel>();

            return layoutsDto;
        }
    }
}
