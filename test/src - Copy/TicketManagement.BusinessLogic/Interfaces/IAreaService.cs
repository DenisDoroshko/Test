using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    /// <summary>
    /// Responsible for proxy all area repository calls with validation logic.
    /// </summary>
    public interface IAreaService
    {
        /// <summary>
        /// Creates area with validation.
        /// </summary>
        /// <param name="areaDto">Area dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> CreateAreaAsync(AreaDto areaDto);

        /// <summary>
        /// Updates area with validation.
        /// </summary>
        /// <param name="areaDto">Area dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> UpdateAreaAsync(AreaDto areaDto);

        /// <summary>
        /// Deletes area with validation.
        /// </summary>
        /// <param name="areaDto">Area dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> DeleteAreaAsync(AreaDto areaDto);

        /// <summary>
        /// Gets area from data storage by specified id.
        /// </summary>
        /// <param name="areaId">Specified id of the area.</param>
        /// <returns>Area dto if the operation was successful otherwise null.</returns>
        Task<AreaDto> GetAreaAsync(int areaId);

        /// <summary>
        /// Gets all areas from data storage.
        /// </summary>
        /// <returns>Area list if the operation was successful otherwise empty area list.</returns>
        List<AreaDto> GetAreas();

        /// <summary>
        /// Gets specified areas.
        /// </summary>
        /// <returns>Specified Area list if the operation was successful otherwise empty areas list.</returns>
        List<AreaDto> GetAreasBy(Expression<Func<Area, bool>> predicate);
    }
}
