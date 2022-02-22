using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Responses;
using Server.Application.Utilities;
using Server.Domain.Models;

namespace Server.Application.Features.UserAccount.Queries
{
    public class GetUserQuery : IRequest<BaseResponse>
    {
        public string Email { get; set; }
    }

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, BaseResponse>
    {
        private UserManager<ApplicationUser> _userManager;

        public GetUserQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<BaseResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            // get the user by email and return it!
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                var error = new ExceptionThrowHelper("Email", "User not found.");
                error.Throw();
            }
            return new BaseResponse
            {
                data = user
            };
        }
    }
}