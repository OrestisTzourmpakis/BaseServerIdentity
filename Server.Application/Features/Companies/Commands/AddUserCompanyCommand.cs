using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.Exceptions;
using Server.Application.Responses;
using Server.Application.Utilities;
using Server.Domain.Models;

namespace Server.Application.Features.Companies.Commands
{
    public class AddUserCompanyCommand : IRequest<BaseResponse>
    {
        public int? CompanyId { get; set; }
        public string Email { get; set; }
        public string OwnerEmail { get; set; }
    }

    public class AddUserCompanyCommandHandler : IRequestHandler<AddUserCompanyCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public AddUserCompanyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<BaseResponse> Handle(AddUserCompanyCommand request, CancellationToken cancellationToken)
        {
            // check if user exists!!
            var user = await _userManager.FindByEmailAsync(request.Email);
            ExceptionThrowHelper validationException; ;
            if (user == null)
            {
                // throw user does not exist!!!
                validationException = new ExceptionThrowHelper("Email", $"User with email: {request.Email} was not found!");
                validationException.Throw();
            }
            Company company = default(Company);

            if (request.CompanyId != null)
            {
                // check if the user is assigned has a company
                company = await _unitOfWork.Companies.GetByIdAsync(c => c.Id == request.CompanyId);
                if (company == null)
                {
                    // throw error that the company was not found with that id!!
                    validationException = new ExceptionThrowHelper("Id", $"Company was not found!");
                    validationException.Throw();
                }
            }
            else if (!string.IsNullOrEmpty(request.OwnerEmail))
            {
                // find the company that this user is the owner for!!
                var userOwnerId = await _userManager.FindByEmailAsync(request.OwnerEmail);
                if (userOwnerId == null)
                {
                    // user owner not found!!
                    validationException = new ExceptionThrowHelper("Email", $"User owner with email :{request.OwnerEmail} was not found!");
                    validationException.Throw();
                }
                company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == userOwnerId.Id);
                if (company == null)
                {
                    // throw error that says that this owner with that email does not have a company!@
                    validationException = new ExceptionThrowHelper("Email", $"Owner with email: {request.Email} does not have a company!");
                    validationException.Throw();
                }

            }
            // check if the user is already in the company!!
            var userAlreadyInCompany = await _unitOfWork.Points.GetByIdAsync(c => c.ApplicationUserId == user.Id && c.CompanyId == company.Id);
            if (userAlreadyInCompany != null)
            {
                // user is already in the company!!
                validationException = new ExceptionThrowHelper("Email", $"User with email: {user.Email} is already assigned to the company!");
                validationException.Throw();
            }
            // user found!! lets add it to our company!!!
            var result = await _unitOfWork.Points.AddAsync(new Domain.Models.Points() { ApplicationUser = user, CompanyId = company.Id });
            // await _unitOfWork.Companies.UpdateAsync(company);
            await _unitOfWork.Save();

            return new BaseResponse()
            {
                data = result
            };
        }
    }
}