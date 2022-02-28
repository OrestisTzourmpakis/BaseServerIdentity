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
    public class GetStoresByCompanyIdQuery : IRequest<BaseResponse>
    {
        public int Id { get; set; }
    }

    public class GetStoresByCompanyIdQueryHandler : IRequestHandler<GetStoresByCompanyIdQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStoresByCompanyIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetStoresByCompanyIdQuery request, CancellationToken cancellationToken)
        {
            var stores = await _unitOfWork.Stores.GetAsync(s => s.CompanyId == request.Id, disableTracking: true);
            return new BaseResponse
            {
                data = stores
            };
        }
    }
}