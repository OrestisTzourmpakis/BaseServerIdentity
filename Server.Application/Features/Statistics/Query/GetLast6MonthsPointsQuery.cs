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
    public class GetLast6MonthsPointsQuery : IRequest<BaseResponse>
    {
        public string CompanyOwnerEmail { get; set; }
    }

    public class GetLast6MonthsPointsQueryHandler : IRequestHandler<GetLast6MonthsPointsQuery, BaseResponse>
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private UserManager<ApplicationUser> _userManager;

        public GetLast6MonthsPointsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<BaseResponse> Handle(GetLast6MonthsPointsQuery request, CancellationToken cancellationToken)
        {
            // find the user and then his company!!!
            var dateStart = DateTime.Now.AddMonths(-5);
            var user = await _userManager.FindByEmailAsync(request.CompanyOwnerEmail);
            var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
            // get the points that this company got!!!
            var points = await _unitOfWork.PointsHistory.GetAsync(c => (c.TransactionDate >= dateStart && c.TransactionDate <= DateTime.Now) && c.CompanyId == company.Id, disableTracking: true);
            // i have all the points need to split them between redeem and earned points!!!
            var month1Redeem = points.Where(c => c.TransactionDate.Month == dateStart.Month && c.Transaction < 0).Select(c => c.Transaction).Sum() * -1;
            var month2Redeem = points.Where(c => c.TransactionDate.Month == dateStart.AddMonths(1).Month && c.Transaction < 0).Select(c => c.Transaction).Sum() * -1;
            var month3Redeem = points.Where(c => c.TransactionDate.Month == dateStart.AddMonths(2).Month && c.Transaction < 0).Select(c => c.Transaction).Sum() * -1;
            var month4Redeem = points.Where(c => c.TransactionDate.Month == dateStart.AddMonths(3).Month && c.Transaction < 0).Select(c => c.Transaction).Sum() * -1;
            var month5Redeem = points.Where(c => c.TransactionDate.Month == dateStart.AddMonths(4).Month && c.Transaction < 0).Select(c => c.Transaction).Sum() * -1;
            var month6Redeem = points.Where(c => c.TransactionDate.Month == DateTime.Now.Month && c.Transaction < 0).Select(c => c.Transaction).Sum() * -1;

            // foreach month how many points the company got!!!
            var month1 = points.Where(c => c.TransactionDate.Month == dateStart.Month && c.Transaction > 0).Select(c => c.Transaction).Sum();
            var month2 = points.Where(c => c.TransactionDate.Month == dateStart.AddMonths(1).Month && c.Transaction > 0).Select(c => c.Transaction).Sum();
            var month3 = points.Where(c => c.TransactionDate.Month == dateStart.AddMonths(2).Month && c.Transaction > 0).Select(c => c.Transaction).Sum();
            var month4 = points.Where(c => c.TransactionDate.Month == dateStart.AddMonths(3).Month && c.Transaction > 0).Select(c => c.Transaction).Sum();
            var month5 = points.Where(c => c.TransactionDate.Month == dateStart.AddMonths(4).Month && c.Transaction > 0).Select(c => c.Transaction).Sum();
            var month6 = points.Where(c => c.TransactionDate.Month == DateTime.Now.Month && c.Transaction > 0).Select(c => c.Transaction).Sum();

            return new BaseResponse
            {
                data = new List<dynamic>{
                    new {Name = dateStart.ToString("MMMM") +" "+ dateStart.ToString("yyyy"),Redeemed = month1Redeem,Earned = month1},
                    new {Name = dateStart.AddMonths(1).ToString("MMMM")+" " + dateStart.AddMonths(1).ToString("yyyy"),Redeemed = month2Redeem,Earned = month2},
                    new {Name = dateStart.AddMonths(2).ToString("MMMM")+" " + dateStart.AddMonths(2).ToString("yyyy"),Redeemed = month3Redeem,Earned = month3},
                    new {Name = dateStart.AddMonths(3).ToString("MMMM")+" " + dateStart.AddMonths(3).ToString("yyyy"),Redeemed = month4Redeem,Earned = month4},
                    new {Name =dateStart.AddMonths(4).ToString("MMMM")+" " + dateStart.AddMonths(4).ToString("yyyy"),Redeemed = month5Redeem,Earned = month5},
                    new {Name =  DateTime.Now.ToString("MMMM")+" " + DateTime.Now.ToString("yyyy"),Redeemed = month6Redeem,Earned = month6},

                }
            };

        }
    }
}