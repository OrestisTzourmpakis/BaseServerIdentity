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

namespace Server.Application.Features.Stores.Queries
{
    public class GetStoreByCompanyQuery : IRequest<BaseResponse>
    {
        public string UserId { get; set; }
    }

    public class GetStoreByCompanyQueryHandler : IRequestHandler<GetStoreByCompanyQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStoreByCompanyQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetStoreByCompanyQuery request, CancellationToken cancellationToken)
        {
            // var stores = await _unitOfWork.Stores.GetAsync(c => c. == request.CompanyId, disableTracking: true);
            var includeList = new List<Expression<Func<Company, object>>>() { c => c.Stores };
            var result = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == request.UserId, includes: includeList);
            return new BaseResponse()
            {
                data = result.Stores
            };
        }
    }
}