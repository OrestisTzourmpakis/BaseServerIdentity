using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Contracts;
using Server.Domain.Models;

namespace Server.Application.Features.UserAccount.Commands.Login
{
    public class GoogleLoginCommand : IRequest<ChallengeResult>
    {
        public string ViewUrl { get; set; }
    }

    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, ChallengeResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;

        public GoogleLoginCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessorWrapper httpContextAccessorWrapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
        }

        public async Task<ChallengeResult> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var serverUrl = _httpContextAccessorWrapper.GetUrl();
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", $"{serverUrl}api/useraccount/externallogin?viewUrl={request.ViewUrl}");
            return new ChallengeResult("Google", properties);
        }
    }
}