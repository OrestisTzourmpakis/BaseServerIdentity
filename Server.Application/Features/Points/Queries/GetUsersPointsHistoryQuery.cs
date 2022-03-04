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
    public class GetUsersPointsHistoryQuery : IRequest<BaseResponse>
    {
        public string Email { get; set; }
    }

    public class GetUsersPointsHistoryQueryHandler : IRequestHandler<GetUsersPointsHistoryQuery, BaseResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public GetUsersPointsHistoryQueryHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetUsersPointsHistoryQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            // find the user then get the user points with include the company
            List<Expression<Func<Server.Domain.Models.PointsHistory, object>>> includeList = new List<Expression<Func<Server.Domain.Models.PointsHistory, object>>>();
            includeList.Add(c => c.Company);
            // get the points
            var points = await _unitOfWork.PointsHistory.GetAsync(c => c.ApplicationUserId == user.Id, orderBy: c => c.OrderByDescending(b => b.TransactionDate), includes: includeList);
            return new BaseResponse
            {
                data = points
            };
        }
    }
}