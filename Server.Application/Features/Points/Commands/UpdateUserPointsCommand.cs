using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.Points.Commands
{
    public class UpdateUserPointsCommand : IRequest<BaseResponse>
    {
        // i want the email
        // and the companyId?
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public int Total { get; set; }
        public string ApplicationUserId { get; set; }
    }


    public class UpdateUserPointsCommandHandler : IRequestHandler<UpdateUserPointsCommand, BaseResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserPointsCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(UpdateUserPointsCommand request, CancellationToken cancellationToken)
        {
            // var user = await _userManager.FindByEmailAsync(request.Email);
            var userPoints = await _unitOfWork.Points.GetByIdAsync(c => c.CompanyId == request.CompanyId && c.ApplicationUserId == request.ApplicationUserId);
            // i have the userpoints now update!!
            var pointsDifference = request.Total - userPoints.Total;
            userPoints.Total = request.Total;
            await _unitOfWork.Points.UpdateAsync(userPoints);
            await _unitOfWork.PointsHistory.AddAsync(new PointsHistory() { ApplicationUserId = request.ApplicationUserId, CompanyId = request.CompanyId, Transaction = pointsDifference });
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = userPoints
            };
        }
    }
}