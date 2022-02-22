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

namespace Server.Application.Features.UserAccount.Queries
{
    public class GetAllUsersByCompanyQuery : IRequest<BaseResponse>
    {
        public string Email { get; set; }
    }

    public class GetAllUsersByCompanyQueryHandler : IRequestHandler<GetAllUsersByCompanyQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersByCompanyQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<BaseResponse> Handle(GetAllUsersByCompanyQuery request, CancellationToken cancellationToken)
        {
            // get all the users by company!!
            var user = await _userManager.FindByEmailAsync(request.Email);
            var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
            if (company == null)
            {
                return new BaseResponse
                {
                    data = new List<Company>()
                };
            }
            var includeList = new List<Expression<Func<Domain.Models.Points, object>>>() { c => c.ApplicationUser };
            var result = (await _unitOfWork.Points.GetAsync(c => c.CompanyId == company.Id, includes: includeList)).Select(c => c.ApplicationUser).ToList();
            return new BaseResponse()
            {
                data = result
            };
        }
    }
}