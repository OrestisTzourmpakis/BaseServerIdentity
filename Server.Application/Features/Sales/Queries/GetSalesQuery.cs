using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Sales.Queries
{
    public class GetSalesQuery : IRequest<BaseResponse>
    {
        public int CompanyId { get; set; }
    }

    public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, BaseResponse>
    {

        private readonly IUnitOfWork _unitOfWork;

        public GetSalesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetSalesQuery request, CancellationToken cancellationToken)
        {
            var sales = await _unitOfWork.Sales.GetAsync(q => q.CompanyId == request.CompanyId, disableTracking: true);
            return new BaseResponse()
            {
                data = sales
            };
        }
    }
}