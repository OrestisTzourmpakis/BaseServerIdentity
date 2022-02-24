using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.Statistics.Query
{
    public class GetTotalEarnedAndRedeemPointsQuery : IRequest<BaseResponse>
    {
        public string CompanyOwnerEmail { get; set; }
    }

    public class GetTotalEarnedAndRedeemPointsQueryHandler : IRequestHandler<GetTotalEarnedAndRedeemPointsQuery, BaseResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTotalEarnedAndRedeemPointsQueryHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(GetTotalEarnedAndRedeemPointsQuery request, CancellationToken cancellationToken)
        {
            // get the date?
            // prwta dei3e generally!!!
            // kai meta ftia3e me date kai timeframe!!
            var user = await _userManager.FindByEmailAsync(request.CompanyOwnerEmail);
            var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
            // the points history!!!
            var pointsHistory = await _unitOfWork.PointsHistory.GetAsync(c => c.CompanyId == company.Id, disableTracking: true);
            var redeemPoints = pointsHistory.Where(c => c.Transaction < 0).Select(c => c.Transaction).Sum() * -1;
            var earnedPoints = pointsHistory.Where(c => c.Transaction > 0).Select(c => c.Transaction).Sum();
            return new BaseResponse
            {
                data = new List<dynamic>{
                    new {
                        name = "Total",
                        redeem = redeemPoints,
                        earned = earnedPoints
                    },
                }
            };

        }
    }
}