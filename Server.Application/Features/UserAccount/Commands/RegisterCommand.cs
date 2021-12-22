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

namespace Server.Application.Features.UserAccount.Commands
{
    public class RegisterCommand : IRequest<BaseResponse>
    {
        public RegisterDto RegisterModel { get; set; }
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
            var result = await _userAccount.RegisterUser(request.RegisterModel);
            return new BaseResponse { data = "User registered successfully!" };
        }
    }
}