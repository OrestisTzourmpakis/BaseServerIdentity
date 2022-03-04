using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Statistics.Query
{
    public class GetTotalCompaniesQuery : IRequest<BaseResponse>
    {

    }

    public class GetTotalCompaniesQueryHandler : IRequestHandler<GetTotalCompaniesQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public async Task<BaseResponse> Handle(GetTotalCompaniesQuery request, CancellationToken cancellationToken)
        {
            var totalCompanies = (await _unitOfWork.Companies.GetAllAsync()).Count();
            return new BaseResponse
            {
                data = totalCompanies
            };
        }
    }
}