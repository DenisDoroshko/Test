using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;
using TicketManagement.EventManagerApi.Models;

namespace TicketManagement.EventManagerApi.Services.Interfaces
{
    /// <summary>
    /// Responsible for proxy layout repository calls.
    /// </summary>
    public interface ILayoutService
    {
        /// <summary>
        /// Gets layout from data storage by specified id.
        /// </summary>
        /// <param name="layoutModelId">Specified id of the layout.</param>
        /// <returns>Layout if the operation was successful otherwise null.</returns>
        Task<LayoutModel> GetLayoutAsync(int layoutModelId);

        /// <summary>
        /// Gets specified layouts.
        /// </summary>
        /// <returns>Specified Layout list if the operation was successful otherwise empty layout list.</returns>
        List<LayoutModel> GetLayoutsBy(Expression<Func<Layout, bool>> predicate);
    }
}
