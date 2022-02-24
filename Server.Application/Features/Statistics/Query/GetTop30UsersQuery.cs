using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class GetTop30UsersQuery : IRequest<BaseResponse>
    {
        public string CompanyOwnerEmail { get; set; }
    }

    public class GetTop30UsersQueryHandler : IRequestHandler<GetTop30UsersQuery, BaseResponse>
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<ApplicationUser> _userManager;
        private IMapper _mapper;
        public GetTop30UsersQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(GetTop30UsersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Server.Domain.Models.Points> final = default(IEnumerable<Server.Domain.Models.Points>);
            List<PointsUserResponse> response = default(List<PointsUserResponse>);
            if (!string.IsNullOrEmpty(request.CompanyOwnerEmail))
            {
                var user = await _userManager.FindByEmailAsync(request.CompanyOwnerEmail);
                var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
                List<Expression<Func<Server.Domain.Models.Points, object>>> includeList = new List<Expression<Func<Domain.Models.Points, object>>>();
                includeList.Add(c => c.ApplicationUser);
                var result = await _unitOfWork.Points.GetAsync(c => c.CompanyId == company.Id, orderBy: c => c.OrderByDescending(a => a.Total), includes: includeList);
                final = result.Take(30);
                response = _mapper.Map<List<PointsUserResponse>>(final);
            }
            else
            {
                // most active users !!
            }

            return new BaseResponse
            {
                data = response
            };
        }
    }
}