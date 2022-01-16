using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Exceptions;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.UserAccount
{
    public class DeleteUserCommand : IRequest<BaseResponse>
    {

        public string Email { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, BaseResponse>
    {
        private UserManager<ApplicationUser> _userManager;

        public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<BaseResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                var failure = new ValidationFailure("Email", $"User with email: {request.Email} was not found!");
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            // delete the user
            await _userManager.DeleteAsync(user);
            return new BaseResponse()
            {
                data = "User deleted successfully"
            };
        }
    }
}