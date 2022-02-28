using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Sales.Queries
{
    public class GetActiveSalesByCompanyIdQuery : IRequest<BaseResponse>
    {
        public int Id { get; set; }
    }

    public class GetActiveSalesByCompanyIdQueryHandler : IRequestHandler<GetActiveSalesByCompanyIdQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public GetActiveSalesByCompanyIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(GetActiveSalesByCompanyIdQuery request, CancellationToken cancellationToken)
        {
            // i have the company id so go to the sales table!!
            var activeSales = await _unitOfWork.Sales.GetAsync(c => c.CompanyId == request.Id && c.DateEnd > DateTime.Now, disableTracking: true);
            return new BaseResponse
            {
                data = activeSales
            };
        }
    }
}