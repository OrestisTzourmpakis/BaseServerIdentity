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

namespace Server.Application.Features.Stores.Queries
{
    public class GetStoreByCompanyQuery : IRequest<BaseResponse>
    {
        public string Email { get; set; }
        public int? CompanyId { get; set; }
    }

    public class GetStoreByCompanyQueryHandler : IRequestHandler<GetStoreByCompanyQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetStoreByCompanyQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<BaseResponse> Handle(GetStoreByCompanyQuery request, CancellationToken cancellationToken)
        {
            // var stores = await _unitOfWork.Stores.GetAsync(c => c. == request.CompanyId, disableTracking: true);
            ICollection<Store> stores = new List<Store>();
            Company result = default(Company);
            var includeList = new List<Expression<Func<Company, object>>>() { c => c.Stores };
            if (!String.IsNullOrWhiteSpace(request.Email))
            {

                var user = await _userManager.FindByEmailAsync(request.Email);
                result = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id, includes: includeList);
                stores = result.Stores;
            }
            else
            {
                if (request.CompanyId == null) throw new Exception("Please pass correct parameters!");
                // i have the company id so get it from there!!
                result = await _unitOfWork.Companies.GetByIdAsync(c => c.Id == request.CompanyId, includes: includeList);
                stores = result.Stores;
            }
            return new BaseResponse()
            {
                data = stores
            };
        }
    }
}