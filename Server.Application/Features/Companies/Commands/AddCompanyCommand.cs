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
    public class AddCompanyCommand : IRequest<BaseResponse>
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Website { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public string ApplicationUserId { get; set; }
        public string Email { get; set; }
        public double EuroToPointsRatio { get; set; }
        public double PointsToEuroRatio { get; set; }
        public int? CategoryId { get; set; }
        public IFormFile LogoFile { get; set; }
    }

    public class AddCompanyCommandHandler : IRequestHandler<AddCompanyCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;

        public AddCompanyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _fileService = fileService;
        }
        public async Task<BaseResponse> Handle(AddCompanyCommand request, CancellationToken cancellationToken)
        {
            // get the email of the user
            var user = await _userManager.FindByEmailAsync(request.Email);
            ExceptionThrowHelper validationException;
            if (user == null)
            {
                validationException = new ExceptionThrowHelper("Email", $"User with email: {request.Email} was not found!");
                validationException.Throw();
            }
            // user found check his role
            var checkRole = await _userManager.IsInRoleAsync(user, "CompanyOwner");
            if (!checkRole)
            {
                validationException = new ExceptionThrowHelper("Role", $"User with email: {request.Email} is not authorized to create a company!");
                validationException.Throw();
            }
            var checkCompany = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
            if (checkCompany != null)
            {
                var failure = new ValidationFailure("Id", $"User with email: {request.Email} is already registered to a company.");
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            if (request.Logo != null && request.LogoFile != null)
                request.Logo = await _fileService.SaveImage(request.LogoFile, $"CompanyImage-{user.UserName}-");
            request.ApplicationUserId = user.Id;
            var company = await _unitOfWork.Companies.AddAsync(_mapper.Map<Company>(request));
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = company
            };
        }
    }
}