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
    public class GetAllCompaniesQuery : IRequest<BaseResponse>
    {

    }

    public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCompaniesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            // get all companies
            var companies = await _unitOfWork.Companies.GetAllAsync();
            return new BaseResponse()
            {
                data = companies
            };
        }
    }
}