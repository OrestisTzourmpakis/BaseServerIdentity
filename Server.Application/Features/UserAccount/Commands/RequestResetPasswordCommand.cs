using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.Utilities;
using Server.Domain.Models;

namespace Server.Application.Features.UserAccount.Commands
{
    public class RequestResetPasswordCommand : IRequest<Unit>
    {
        public string Email { get; set; }
    }

    public class RequestResetPasswordCommandHandler : IRequestHandler<RequestResetPasswordCommand, Unit>
    {
        private readonly IUserAccount _userAccount;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;

        public RequestResetPasswordCommandHandler(IUserAccount userAccount, IEmailSender emailSender, IHttpContextAccessorWrapper httpContextAccessorWrapper)
        {
            _userAccount = userAccount;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
            _emailSender = emailSender;
        }

        public async Task<Unit> Handle(RequestResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // get the token
            var token = await _userAccount.GenerateResetPasswordToken(request.Email);
            // the callback!!
            var dict = new Dictionary<string, string>(){
            {
                "token",token
            },
            {
                "email",request.Email
            }
            };
            var url = _httpContextAccessorWrapper.ConstructUrl("api/useraccount/resetPassword", dict);
            _emailSender.SendEmaiForgotPassowrdLink(request.Email, "Reset email", url);
            return Unit.Value;
        }
    }
}