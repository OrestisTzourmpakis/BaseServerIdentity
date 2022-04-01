using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<BaseResponse>
    {

    }

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Categories.GetAllAsync();
            return new BaseResponse
            {
                data = result
            };
        }
    }
}