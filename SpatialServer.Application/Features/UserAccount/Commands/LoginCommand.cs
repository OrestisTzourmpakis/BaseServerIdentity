using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SpatialServer.Application.Contracts;
using SpatialServer.Application.DTO.Login;
using SpatialServer.Application.DTO.Register;
using SpatialServer.Application.Responses;

namespace SpatialServer.Application.Features.UserAccount.Commands
{
    public class LoginCommand : IRequest<BaseResponse>
    {
        public LoginDto LoginModel { get; set; }
    }
    public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseResponse>
    {
        private readonly IUserAccount _userAccount;

        public LoginCommandHandler(IUserAccount userAccount)
        {
            _userAccount = userAccount;
        }

        public async Task<BaseResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _userAccount.LoginUser(request.LoginModel);
            var response = new BaseResponse { data = "User logged in successfully!" };
            return response;
        }
    }
}
