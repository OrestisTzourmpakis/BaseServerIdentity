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
        public int Id { get; set; }
    }
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;

        public GetCompanyByIdQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessorWrapper httpContextAccessorWrapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
        }

        public async Task<BaseResponse> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(c => c.Id == request.Id);
            if (company == null)
            {
                var failure = new ValidationFailure("Id", $"Company was not found.");
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            var finalUrl = _httpContextAccessorWrapper.GetUrl();
            finalUrl += "Images/";
            company.Logo = company.Logo == null ? null : finalUrl + company.Logo;
            return new BaseResponse() { data = company };


        }
    }
}