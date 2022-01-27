using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Server.Application.Contracts;
using Server.Application.DTO.Register;
using Server.Application.Responses;

namespace Server.Application.Features.UserAccount.Commands.Register
{
    public class RegisterCommand : IRequest<BaseResponse>
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Owner { get; set; } = false;
    }
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, BaseResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserAccount _userAccount;

        public RegisterCommandHandler(IMapper mapper, IUserAccount userAccount)
        {
            _mapper = mapper;
            _userAccount = userAccount;
        }

        public async Task<BaseResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = await _userAccount.RegisterUser(request);
            return new BaseResponse { data = "User registered successfully!" };
        }
    }
}