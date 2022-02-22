using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.Utilities;
using Server.Domain.Models;

namespace Server.Application.Features.Companies.Queries
{
    public class GetCompanyByEmailQuery : IRequest<Company>
    {
        public string Email { get; set; }
    }

    public class GetCompanyByEmailQueryHandler : IRequestHandler<GetCompanyByEmailQuery, Company>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;


        public GetCompanyByEmailQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessorWrapper httpContextAccessorWrapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
        }

        public async Task<Company> Handle(GetCompanyByEmailQuery request, CancellationToken cancellationToken)
        {
            ExceptionThrowHelper error = default(ExceptionThrowHelper);
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                error = new ExceptionThrowHelper("Email", $"User was not found");
                error.Throw();
            }
            var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
            if (company == null)
            {
                error = new ExceptionThrowHelper("Company", $"Company was not found.");
                error.Throw();
            }
            var finalUrl = _httpContextAccessorWrapper.GetUrl() + "Images/";
            company.Logo = company.Logo == null ? null : finalUrl + company.Logo;
            return company;
        }
    }
}