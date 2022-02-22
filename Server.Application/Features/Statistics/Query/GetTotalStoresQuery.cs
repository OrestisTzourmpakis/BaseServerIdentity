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
    public class GetTotalStoresQuery : IRequest<BaseResponse>
    {
        public string CompanyOwnerEmail { get; set; }
    }

    public class GetTotalStoresQueryHandler : IRequestHandler<GetTotalStoresQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetTotalStoresQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<BaseResponse> Handle(GetTotalStoresQuery request, CancellationToken cancellationToken)
        {
            int totalStores = default(int);
            if (!string.IsNullOrEmpty(request.CompanyOwnerEmail))
            {

                var user = await _userManager.FindByEmailAsync(request.CompanyOwnerEmail);
                List<Expression<Func<Company, object>>> includeList = new List<Expression<Func<Company, object>>>();
                includeList.Add(c => c.Stores);
                var result = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id, includes: includeList);
                totalStores = result.Stores.Count();
            }
            else
                totalStores = (await _unitOfWork.Stores.GetAllAsync()).Count();
            return new BaseResponse
            {
                data = totalStores
            };
        }
    }
}