using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Server.Application.Features.UserAccount.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("Please enter your {Email}.");
            RuleFor(p => p.Password)
                .NotEmpty().WithMessage("Please enter your {Password}.")
                .NotNull()
                .MinimumLength(8).WithMessage("{Password} must be at least 8 character long.");

        }
    }
}