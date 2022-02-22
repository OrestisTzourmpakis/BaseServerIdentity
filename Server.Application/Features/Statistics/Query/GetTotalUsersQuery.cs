using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Application.Contracts;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.Statistics.Query
{
    public class GetTotalUsersQuery : IRequest<BaseResponse>
    {
        public string CompanyOwnerEmail { get; set; }
    }

    public class GetTotalUsersQueryHandler : IRequestHandler<GetTotalUsersQuery, BaseResponse>
    {
        private UserManager<ApplicationUser> _userManager;
        private IUnitOfWork _unitOfWork;

        public GetTotalUsersQueryHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetTotalUsersQuery request, CancellationToken cancellationToken)
        {

            int totalUsers = default(int);
            if (string.IsNullOrEmpty(request.CompanyOwnerEmail))
            {

                totalUsers = await _userManager.Users.CountAsync();
            }
            else
            {
                // get first the company id that correspond to the user
                var owner = await _userManager.FindByEmailAsync(request.CompanyOwnerEmail);
                var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == owner.Id);
                totalUsers = (await _unitOfWork.Points.GetAsync(c => c.CompanyId == company.Id, disableTracking: true)).Count();
            }
            return new BaseResponse
            {
                data = totalUsers
            };
        }
    }
}