using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Server.Application.Contracts;
using Server.Application.Extensions;
using Server.Application.Utilities;

namespace Server.Application.Features.UserAccount.Commands
{
    public class VerifyEmailCommand : IRequest<ViewResult>
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public ViewDataDictionary ViewData { get; set; }
    }

    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, ViewResult>
    {
        private readonly IUserAccount _userAccount;
        private readonly IMapper _mapper;

        public VerifyEmailCommandHandler(IUserAccount userAccount, IMapper mapper)
        {
            _userAccount = userAccount;
            _mapper = mapper;
        }

        public async Task<ViewResult> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await _userAccount.ConfirmEmail(request.Token, request.Email);
            var htmlView = new HtmlView(request.ViewData);
            return htmlView.GetViewResult(HtmlTemplates.ConfirmationEmailView, new { Success = result }.ToExpando());
        }
    }
}