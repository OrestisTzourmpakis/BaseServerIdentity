using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.Exceptions;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.Companies.Queries
{
    public class GetCompanyByIdQuery : IRequest<BaseResponse>
    {
        public string Email { get; set; }
    }
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetCompanyByIdQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<BaseResponse> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
            if (company == null)
            {
                var failure = new ValidationFailure("Id", $"User with email : {request.Email} does not have a company.");
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            return new BaseResponse() { data = company };


        }
    }
}