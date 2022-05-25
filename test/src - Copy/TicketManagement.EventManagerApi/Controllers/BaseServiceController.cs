using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace TicketManagement.EventManagerApi.Controllers
{
    public class BaseServiceController : Controller
    {
        protected ValidationResult GetValidationResultWithError(string errorMessage)
        {
            var result = new ValidationResult(new List<ValidationFailure>());
            result.Errors.Add(new ValidationFailure("", errorMessage));
            return result;
        }
    }
}
