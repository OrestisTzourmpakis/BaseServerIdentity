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
using Server.Domain.Models;

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
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;

        public GetActiveSalesByCompanyIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessorWrapper httpContextAccessorWrapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
        }

        public async Task<BaseResponse> Handle(GetActiveSalesByCompanyIdQuery request, CancellationToken cancellationToken)
        {
            // i have the company id so go to the sales table!!
            var activeSales = await _unitOfWork.Sales.GetAsync(c => c.CompanyId == request.Id && c.DateEnd > DateTime.Now, disableTracking: true);
            var includeListCategory = new List<Expression<Func<Company, object>>>() { c => c.Category };

            var category = await _unitOfWork.Companies.GetByIdAsync(c => c.Id == request.Id, includes: includeListCategory);
            var finalCategory = category.Category != null ? category.Category.Name : null;

            var finalSales = activeSales.Select(a =>
            {
                a.Image = a.Image == null ? null : _httpContextAccessorWrapper.GetUrl() + "Images/" + a.Image;
                a.Category = finalCategory;
                return a;
            });
            return new BaseResponse
            {
                data = finalSales
            };
        }
    }
}