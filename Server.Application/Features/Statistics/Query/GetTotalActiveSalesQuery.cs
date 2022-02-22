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

namespace Server.Application.Features.Statistics.Query
{
    public class GetTotalActiveSalesQuery : IRequest<BaseResponse>
    {
        public string CompanyOwnerEmail { get; set; }
    }

    public class GetTotalActiveSalesQueryHandler : IRequestHandler<GetTotalActiveSalesQuery, BaseResponse>
    {
        private UserManager<ApplicationUser> _userManager;
        private IUnitOfWork _unitOfWork;

        public GetTotalActiveSalesQueryHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetTotalActiveSalesQuery request, CancellationToken cancellationToken)
        {
            int totalActiveSales = default(int);
            if (!string.IsNullOrEmpty(request.CompanyOwnerEmail))
            {
                var user = await _userManager.FindByEmailAsync(request.CompanyOwnerEmail);
                List<Expression<Func<Company, object>>> includeList = new List<Expression<Func<Company, object>>>();
                includeList.Add(c => c.Sales);
                var result = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id, includes: includeList);
                totalActiveSales = result.Sales.Where(c => c.DateEnd > DateTime.Now).Count();
            }
            else
                totalActiveSales = (await _unitOfWork.Sales.GetAsync(c => c.DateEnd > DateTime.Now, disableTracking: true)).Count();
            return new BaseResponse
            {
                data = totalActiveSales
            };
        }
    }
}