using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Points.Commands
{
    public class AddUserPointsCommand : IRequest<BaseResponse>
    {
        public int CompanyId { get; set; }
        public string UserId { get; set; }
        public double Points { get; set; }
    }
    public class AddUserPointsCommandHandler : IRequestHandler<AddUserPointsCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddUserPointsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(AddUserPointsCommand request, CancellationToken cancellationToken)
        {
            // update the points first and then add to the history!!!
            var userPoints = await _unitOfWork.Points.GetByIdAsync(c => c.ApplicationUserId == request.UserId && c.CompanyId == request.CompanyId);
            userPoints.Total = userPoints.Total + request.Points;
            await _unitOfWork.Points.UpdateAsync(userPoints);
            await _unitOfWork.PointsHistory.AddAsync(new Domain.Models.PointsHistory() { ApplicationUserId = request.UserId, CompanyId = request.CompanyId, Transaction = request.Points });
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = userPoints
            };
        }
    }
}