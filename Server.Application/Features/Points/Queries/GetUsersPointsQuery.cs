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
    public class GetUsersPointsQuery : IRequest<BaseResponse>
    {
        public string Email { get; set; }
    }

    public class GetUsersPointsQueryHandler : IRequestHandler<GetUsersPointsQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUsersPointsQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<BaseResponse> Handle(GetUsersPointsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            // find the user then get the user points with include the company
            List<Expression<Func<Server.Domain.Models.Points, object>>> includeList = new List<Expression<Func<Server.Domain.Models.Points, object>>>();
            includeList.Add(c => c.Company);
            // get the points
            var points = await _unitOfWork.Points.GetAsync(c => c.ApplicationUserId == user.Id, includes: includeList);
            return new BaseResponse
            {
                data = points
            };

        }
    }
}