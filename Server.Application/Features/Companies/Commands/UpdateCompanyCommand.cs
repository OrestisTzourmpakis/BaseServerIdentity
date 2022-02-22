using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.Exceptions;
using Server.Application.Responses;
using Server.Application.Utilities;
using Server.Domain.Models;

namespace Server.Application.Features.Companies.Commands
{
    public class UpdateCompanyCommand : IRequest<BaseResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Website { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public string OwnerEmail { get; set; }
        public double EuroToPointsRatio { get; set; }
        public double PointsToEuroRatio { get; set; }
        public string ApplicationUserId { get; set; }
        public IFormFile LogoFile { get; set; }
    }
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;

        public UpdateCompanyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _fileService = fileService;
        }

        public async Task<BaseResponse> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            // find the company first
            var includeList = new List<Expression<Func<Company, object>>>() { c => c.Owner };
            var company = await _unitOfWork.Companies.GetByIdAsync(c => c.Id == request.Id, includes: includeList);
            if (company == null)
            {
                var failure = new ValidationFailure("Id", $"Company with id: {request.Id} does not exist!");
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            // check the current user if its the same
            // if (company.Owner == null || company.Owner == default(ApplicationUser)) { 

            // }   
            // yparxei owner ara tsekare to id ama einai to idio me to updated!!
            if (company.Owner == null || !company.Owner.Email.Equals(request.OwnerEmail.Trim()))
            {
                // i have changed the user
                var newOwnerEmail = await _userManager.FindByEmailAsync(request.OwnerEmail.Trim());
                if (newOwnerEmail == null)
                {
                    var newOwnerEmailError = new ExceptionThrowHelper("Email", $"User with email:{request.OwnerEmail.Trim()} was not found!");
                    newOwnerEmailError.Throw();
                }
                else
                {
                    // user found check first if :
                    // 1    Has companyowner role
                    // 2    Does not have a company
                    var newOwnerEmailRoles = await _userManager.GetRolesAsync(newOwnerEmail);
                    if (newOwnerEmailRoles.Contains(UserRoles.CompanyOwner.ToString()))
                    {
                        // the user is company owner
                        // check if it has a company!
                        var checkCompany = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == newOwnerEmail.Id);
                        if (checkCompany != null)
                        {
                            // throw error user has already registered as an owner to a company
                            var alreadyOwnerError = new ExceptionThrowHelper("Email", $"User with email:{request.OwnerEmail.Trim()} is already an owner to a company!");
                            alreadyOwnerError.Throw();
                        }
                        // the user is not an owner to the company!!
                        // assign its application id!
                        request.ApplicationUserId = newOwnerEmail.Id;
                    }
                    else
                    {
                        // the user is not a company owner please select a user that has a company owner role!!
                        var userNotAnOwnerError = new ExceptionThrowHelper("Email", $"User with email:{request.OwnerEmail.Trim()} has not an owner role!");
                        userNotAnOwnerError.Throw();
                    }
                }
            }
            else
            {
                request.ApplicationUserId = company.ApplicationUserId;
            }
            if (request.LogoFile == null)
                request.Logo = company.Logo;
            else
                request.Logo = await _fileService.SaveImage(request.LogoFile, $"CompanyImage-{company.Id}-");
            // exists update it
            _mapper.Map(request, company);
            await _unitOfWork.Companies.UpdateAsync(company);
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = company
            };
        }
    }
}