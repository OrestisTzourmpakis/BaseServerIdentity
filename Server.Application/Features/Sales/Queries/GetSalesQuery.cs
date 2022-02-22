using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.Sales.Queries
{
    public class GetSalesQuery : IRequest<BaseResponse>
    {
        public string Email { get; set; }
        public int Id { get; set; }
    }

    public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, BaseResponse>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;

        public GetSalesQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessorWrapper httpContextAccessorWrapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
        }

        public async Task<BaseResponse> Handle(GetSalesQuery request, CancellationToken cancellationToken)
        {
            List<Domain.Models.Sales> finalSales = new List<Domain.Models.Sales>();
            var finaluRL = _httpContextAccessorWrapper.GetUrl();
            finaluRL += "Images/";
            if (!string.IsNullOrEmpty(request.Email))
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                var includeList = new List<Expression<Func<Company, object>>>() { c => c.Sales };
                var result = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id, includes: includeList);
                if (result != null)
                    finalSales = result.Sales.Select(c => { c.Image = c.Image == null ? null : finaluRL + c.Image; return c; }).ToList();
            }
            else
            {
                // parta me to company id!!!
                var resultTemp = await _unitOfWork.Sales.GetAsync(c => c.CompanyId == request.Id, disableTracking: true);
                finalSales = resultTemp.Select(c => { c.Image = c.Image == null ? null : finaluRL + c.Image; return c; }).ToList();
            }
            return new BaseResponse()
            {
                data = finalSales
            };
        }
    }
}