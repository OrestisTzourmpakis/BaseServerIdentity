using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Server.Application.Features.UserAccount.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("{Email} is required.");

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage("{Password} is required.")
                .NotNull()
                .MinimumLength(8).WithMessage("{Password} must be at least 8 character long.");
        }
    }
}