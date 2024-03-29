using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Server.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public List<string> Errors { get; set; } = new List<string>();
        public ValidationException(IEnumerable<ValidationFailure> validationResults)
        {
            foreach (var validationResult in validationResults)
            {
                Errors.Add(validationResult.ErrorMessage);
            }
        }
        public ValidationException(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                Errors.Add(error.Description);
            }
        }
    }
}