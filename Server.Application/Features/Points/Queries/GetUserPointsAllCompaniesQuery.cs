using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.Exceptions;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.Points.Queries
{
    public class GetUserPointsAllCompaniesQuery : IRequest<BaseResponse>
    {
        public string Email { get; set; }
    }

    public class GetUserPointsAllCompaniesQueryHandler : IRequestHandler<GetUserPointsAllCompaniesQuery, BaseResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public GetUserPointsAllCompaniesQueryHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetUserPointsAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                var failure = new ValidationFailure("Email", $"User with email: {request.Email} was not found.");
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            // user found
            var includeList = new List<Expression<Func<Domain.Models.Points, object>>>() { c => c.Company };
            var result = await _unitOfWork.Points.GetAsync(c => c.ApplicationUserId == user.Id, includes: includeList);
            return new BaseResponse()
            {
                data = result
            };
        }
    }
}