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
    public class GetAllStoresQuery : IRequest<BaseResponse>
    {

    }
    public class GetAllStoresQueryHandler : IRequestHandler<GetAllStoresQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllStoresQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetAllStoresQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Stores.GetAllAsync();
            return new BaseResponse()
            {
                data = result
            };
        }
    }
}