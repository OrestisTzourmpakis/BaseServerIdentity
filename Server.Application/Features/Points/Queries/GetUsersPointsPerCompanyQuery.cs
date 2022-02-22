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

namespace Server.Application.Features.Points.Queries
{
    public class GetUsersPointsPerCompanyQuery : IRequest<BaseResponse>
    {
        public string Email { get; set; }
        public string CompanyOwnerEmail { get; set; }
    }

    public class GetUsersPointsPerCompanyQueryHandler : IRequestHandler<GetUsersPointsPerCompanyQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUsersPointsPerCompanyQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<BaseResponse> Handle(GetUsersPointsPerCompanyQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            var companyOwner = await _userManager.FindByEmailAsync(request.CompanyOwnerEmail);
            var comapny = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == companyOwner.Id);
            var includeList = new List<Expression<Func<Domain.Models.Points, object>>>() { c => c.Company };
            var result = await _unitOfWork.Points.GetAsync(c => c.ApplicationUserId == user.Id && c.CompanyId == comapny.Id, includes: includeList);
            return new BaseResponse()
            {
                data = result
            };
        }
    }


}