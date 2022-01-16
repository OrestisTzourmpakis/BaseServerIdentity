using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.UserAccount.Queries
{
    public class GetAllUsersQuery : IRequest<BaseResponse>
    {

    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Users.GetAllAsync();
            return new BaseResponse() { data = result };
        }
    }
}