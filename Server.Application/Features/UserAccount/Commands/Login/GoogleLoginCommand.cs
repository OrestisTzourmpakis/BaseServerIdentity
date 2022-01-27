using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Domain.Models;

namespace Server.Application.Features.UserAccount.Commands.Login
{
    public class GoogleLoginCommand : IRequest<ChallengeResult>
    {

    }

    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, ChallengeResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public GoogleLoginCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ChallengeResult> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", "https://localhost:4004/api/useraccount/externallogin");
            //var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", "http://localhost:3000/users");

            return new ChallengeResult("Google", properties);
        }
    }
}