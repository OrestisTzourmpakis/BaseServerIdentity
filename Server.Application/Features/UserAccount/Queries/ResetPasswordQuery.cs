using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Server.Application.Extensions;
using Server.Application.Models.Identity;
using Server.Application.Utilities;
using Server.Domain.Models;

namespace Server.Application.Features.UserAccount.Queries
{
    public class ResetPasswordQuery : IRequest<ViewResult>
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public ViewDataDictionary ViewData { get; set; }
    }

    public class ResetPasswordQueryHandler : IRequestHandler<ResetPasswordQuery, ViewResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public ResetPasswordQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ViewResult> Handle(ResetPasswordQuery request, CancellationToken cancellationToken)
        {
            // check if the user exists!!
            // error object
            var htmlView = new HtmlView(request.ViewData);
            string errorMessage = "";
            dynamic obj = new ExpandoObject();
            if (request.Email != null && request.Token != null)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user != null)
                {
                    return htmlView.GetViewResult(HtmlTemplates.ResetPasswordForm, _mapper.Map<ResetPasswordModel>(request));
                }
                else
                {
                    errorMessage = "User not found";
                }
            }
            else
            {
                errorMessage = "Something happend please try again later...";
            }
            return htmlView.GetViewResult(HtmlTemplates.ResetPasswordFail, new { Message = errorMessage }.ToExpando());
        }
    }
}