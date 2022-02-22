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
    public class GetTop30UsersQuery : IRequest<BaseResponse>
    {
        public string CompanyOwnerEmail { get; set; }
    }

    public class GetTop30UsersQueryHandler : IRequestHandler<GetTop30UsersQuery, BaseResponse>
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<ApplicationUser> _userManager;
        public GetTop30UsersQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<BaseResponse> Handle(GetTop30UsersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Server.Domain.Models.Points> final = default(IEnumerable<Server.Domain.Models.Points>);
            if (!string.IsNullOrEmpty(request.CompanyOwnerEmail))
            {
                var user = await _userManager.FindByEmailAsync(request.CompanyOwnerEmail);
                var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
                List<Expression<Func<Server.Domain.Models.Points, object>>> includeList = new List<Expression<Func<Domain.Models.Points, object>>>();
                includeList.Add(c => c.ApplicationUser);
                var result = await _unitOfWork.Points.GetAsync(c => c.CompanyId == company.Id, orderBy: c => c.OrderByDescending(a => a.Total), includes: includeList);
                final = result.Take(30);
            }

            return new BaseResponse
            {
                data = final
            };
        }
    }
}