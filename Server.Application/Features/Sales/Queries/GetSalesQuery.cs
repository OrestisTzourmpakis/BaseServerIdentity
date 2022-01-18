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
    }

    public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, BaseResponse>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetSalesQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<BaseResponse> Handle(GetSalesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            var includeList = new List<Expression<Func<Company, object>>>() { c => c.Sales };
            var result = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id, includes: includeList);
            return new BaseResponse()
            {
                data = result.Sales
            };
        }
    }
}