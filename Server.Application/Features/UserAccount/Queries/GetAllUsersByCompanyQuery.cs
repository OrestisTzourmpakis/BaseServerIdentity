using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.UserAccount.Queries
{
    public class GetAllUsersByCompanyQuery : IRequest<BaseResponse>
    {
        public int CompanyId { get; set; }
    }

    public class GetAllUsersByCompanyQueryHandler : IRequestHandler<GetAllUsersByCompanyQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllUsersByCompanyQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetAllUsersByCompanyQuery request, CancellationToken cancellationToken)
        {
            // get all the users by company!!
            var includeList = new List<Expression<Func<Domain.Models.Points, object>>>() { c => c.ApplicationUser };
            var result = await _unitOfWork.Points.GetAsync(c => c.CompanyId == request.CompanyId, includes: includeList);
            return new BaseResponse()
            {
                data = result
            };
        }
    }
}