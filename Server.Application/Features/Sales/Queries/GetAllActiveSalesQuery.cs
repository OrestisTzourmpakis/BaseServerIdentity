using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Sales.Queries
{
    public class GetAllActiveSalesQuery : IRequest<BaseResponse>
    {

    }

    public class GetAllActiveSalesQueryHandler : IRequestHandler<GetAllActiveSalesQuery, BaseResponse>
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;

        public GetAllActiveSalesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessorWrapper httpContextAccessorWrapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
        }

        public async Task<BaseResponse> Handle(GetAllActiveSalesQuery request, CancellationToken cancellationToken)
        {
            List<Expression<Func<Server.Domain.Models.Sales, object>>> includeList = new List<Expression<Func<Server.Domain.Models.Sales, object>>>();
            includeList.Add(c => c.Company);
            var sales = await _unitOfWork.Sales.GetAsync(c => c.DateEnd > DateTime.Now, includes: includeList);
            var finalSales = sales.Select(a =>
            {
                a.Image = a.Image == null ? null : _httpContextAccessorWrapper.GetUrl() + "Images/" + a.Image;
                return a;
            });
            return new BaseResponse
            {
                data = finalSales
            };
        }
    }
}