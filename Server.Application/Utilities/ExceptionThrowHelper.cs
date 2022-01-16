using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using Server.Application.Exceptions;

namespace Server.Application.Utilities
{
    public class ExceptionThrowHelper
    {
        private ValidationFailure _failure;
        public ExceptionThrowHelper(string propertyName, string message)
        {
            _failure = new ValidationFailure(propertyName, message);
        }

        public void Throw()
        {
            throw new ValidationException(new List<ValidationFailure>() { this._failure });
        }
    }
}