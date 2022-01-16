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
    public class GetAllSalesQuery : IRequest<BaseResponse>
    {

    }

    public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public async Task<BaseResponse> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        {
            var sales = await _unitOfWork.Sales.GetAllAsync();
            return new BaseResponse()
            {
                data = sales
            };
        }
    }
}