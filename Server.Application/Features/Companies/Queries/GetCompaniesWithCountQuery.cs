using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Companies.Queries
{
    public class GetCompaniesWithCountQuery : IRequest<BaseResponse>
    {

    }

    public class GetCompaniesWithCountQueryHandler : IRequestHandler<GetCompaniesWithCountQuery, BaseResponse>
    {
        public IUnitOfWork _unitOfWork { get; set; }

        public GetCompaniesWithCountQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetCompaniesWithCountQuery request, CancellationToken cancellationToken)
        {
            return new BaseResponse()
            {
                data = await _unitOfWork.Companies.GetComapniesWithCount()
            };
        }
    }
}