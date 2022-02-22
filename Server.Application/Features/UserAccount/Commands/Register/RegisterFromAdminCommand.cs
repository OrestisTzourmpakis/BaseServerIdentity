using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.UserAccount.Commands.Register
{
    public class RegisterFromAdminCommand : IRequest<BaseResponse>
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Owner { get; set; } = false;
        public string CompanyOwnerEmail { get; set; }
    }

    public class RegisterFromAdminCommandHandler : IRequestHandler<RegisterFromAdminCommand, BaseResponse>
    {
        private readonly IUserAccount _userAccount;

        public RegisterFromAdminCommandHandler(IUserAccount userAccount)
        {
            _userAccount = userAccount;
        }

        public async Task<BaseResponse> Handle(RegisterFromAdminCommand request, CancellationToken cancellationToken)
        {
            var result = await _userAccount.RegisterUserFromAdmin(request);
            return new BaseResponse { data = "User registered successfully!" };
        }
    }
}