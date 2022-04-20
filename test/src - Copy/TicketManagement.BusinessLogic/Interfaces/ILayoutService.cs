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
    /// Responsible for proxy all layout repository calls with validation logic.
    /// </summary>
    public interface ILayoutService
    {
        /// <summary>
        /// Creates layout with validation.
        /// </summary>
        /// <param name="layoutDto">Layout dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> CreateLayoutAsync(LayoutDto layoutDto);

        /// <summary>
        /// Updates layout with validation.
        /// </summary>
        /// <param name="layoutDto">Layout dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> UpdateLayoutAsync(LayoutDto layoutDto);

        /// <summary>
        /// Deletes layout with validation.
        /// </summary>
        /// <param name="layoutDto">Layout dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> DeleteLayoutAsync(LayoutDto layoutDto);

        /// <summary>
        /// Gets layout from data storage by specified id.
        /// </summary>
        /// <param name="layoutId">Specified id of the layout.</param>
        /// <returns>Layout dto if the operation was successful otherwise null.</returns>
        Task<LayoutDto> GetLayoutAsync(int layoutId);

        /// <summary>
        /// Gets all layouts from data storage.
        /// </summary>
        /// <returns>Layout list if the operation was successful otherwise empty layout list.</returns>
        List<LayoutDto> GetLayouts();

        /// <summary>
        /// Gets specified layouts.
        /// </summary>
        /// <returns>Specified Layout list if the operation was successful otherwise empty layout list.</returns>
        List<LayoutDto> GetLayoutsBy(Expression<Func<Layout, bool>> predicate);
    }
}