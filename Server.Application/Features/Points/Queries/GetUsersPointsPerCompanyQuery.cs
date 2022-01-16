using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Points.Queries
{
    public class GetUsersPointsPerCompanyQuery : IRequest<BaseResponse>
    {
        public int CompanyId { get; set; }
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
            var includesList = new List<Expression<Func<Domain.Models.Points, object>>>() { c => c.ApplicationUser };
            var result = await _unitOfWork.Points.GetAsync(c => c.CompanyId == request.CompanyId, includes: includesList);
            return new BaseResponse()
            {
                data = result
            };
        }
    }


}