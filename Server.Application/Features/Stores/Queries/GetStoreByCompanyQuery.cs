using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Stores.Queries
{
    public class GetStoreByCompanyQuery : IRequest<BaseResponse>
    {
        public int CompanyId { get; set; }
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
            var stores = await _unitOfWork.Stores.GetAsync(c => c.CompanyId == request.CompanyId, disableTracking: true);
            return new BaseResponse()
            {
                data = stores
            };
        }
    }
}