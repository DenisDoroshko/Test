using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace TicketManagement.BusinessLogic.Interfaces
{
    /// <summary>
    /// Base service for services with try validation method.
    /// </summary>
    /// <typeparam name="T">Dto class.</typeparam>
    public abstract class BaseService<T>
    {
        /// <summary>
        /// Validator for T class.
        /// </summary>
        protected abstract AbstractValidator<T> Validator { get; set; }

        /// <summary>
        /// Validates entity with catching exceptions.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="ruleSetName">Name of the rule set.</param>
        /// <returns>Validation reuslt.</returns>
        public async Task<ValidationResult> TryValidateAsync(T entity, string ruleSetName)
        {
            ValidationResult result = null;
            try
            {
                result = await Validator.ValidateAsync(entity, options => options.IncludeRuleSets(ruleSetName));
            }
            catch (Exception exception)
            {
                result ??= new ValidationResult(new List<ValidationFailure>());
                result.Errors.Add(new ValidationFailure("", exception.Message));
            }

            return result;
        }
    }
}
