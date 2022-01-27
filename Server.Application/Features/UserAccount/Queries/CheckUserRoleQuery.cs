using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Utilities;
using Server.Domain.Models;

namespace Server.Application.Features.UserAccount.Queries
{
    public class CheckUserRoleQuery : IRequest<bool>
    {
        public string Email { get; set; }

    }

    public class CheckUserRoleQueryHandler : IRequestHandler<CheckUserRoleQuery, bool>
    {
        private UserManager<ApplicationUser> _userManager;

        public CheckUserRoleQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(CheckUserRoleQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                var userNotFoundError = new ExceptionThrowHelper("Email", $"User with email:{request.Email} was not found!");
                userNotFoundError.Throw();
            }
            var check = await _userManager.IsInRoleAsync(user, UserRoles.CompanyOwner.ToString());
            return check;
        }
    }
}