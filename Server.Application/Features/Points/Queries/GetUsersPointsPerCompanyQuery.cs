using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.Points.Queries
{
    public class GetUsersPointsPerCompanyQuery : IRequest<BaseResponse>
    {
        public string UserId { get; set; }
    }

    public class GetUsersPointsPerCompanyQueryHandler : IRequestHandler<GetUsersPointsPerCompanyQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUsersPointsPerCompanyQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetUsersPointsPerCompanyQuery request, CancellationToken cancellationToken)
        {
            var includesList = new List<Expression<Func<Company, object>>>() { c => c.Points };
            var result = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == request.UserId, includes: includesList);
            return new BaseResponse()
            {
                data = result.Points
            };
        }
    }


}