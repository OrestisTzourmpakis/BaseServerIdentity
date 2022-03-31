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
using Server.Application.Utilities;
using Server.Domain.Models;

namespace Server.Application.Features.UserAccount.Commands.Login
{
    public class ExternalLoginCommand : IRequest<ChallengeResult>
    {
        public string ViewUrl { get; set; }
        public ProviderNames Provider { get; set; }
    }

    public class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, ChallengeResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;

        public ExternalLoginCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessorWrapper httpContextAccessorWrapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
        }

        public async Task<ChallengeResult> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
        {
            string providerString = request.Provider.ToString();
            var serverUrl = _httpContextAccessorWrapper.GetUrl();
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(providerString, $"{serverUrl}api/useraccount/externallogin?viewUrl={request.ViewUrl}");
            return new ChallengeResult(providerString, properties);
        }
    }
}