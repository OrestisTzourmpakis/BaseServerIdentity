using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Server.Application.Contracts;

namespace Server.Application.Features.UserAccount.Commands
{
    public class SendEmailCommand : IRequest<Unit>
    {
        public string Email { get; set; }
        public string Topic { get; set; }
        public string Message { get; set; }
    }

    public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, Unit>
    {
        private readonly IEmailSender _emailSender;

        public SendEmailCommandHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<Unit> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            _emailSender.SendEmailFromWebSite(request.Topic, request.Message, request.Email);
            return Unit.Value;
        }
    }
}