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
using Server.Domain.Models;

namespace Server.Application.Features.Companies.Commands
{
    public class AddUserCompanyCommand : IRequest<BaseResponse>
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
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
            if (user == null)
            {
                var failure = new ValidationFailure("Email", $"User with email: {request.Email} was not found!");
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            // user found!! lets add it to our company!!!
            var result = await _unitOfWork.Points.AddAsync(new Domain.Models.Points() { ApplicationUser = user, CompanyId = request.Id });
            // await _unitOfWork.Companies.UpdateAsync(company);
            await _unitOfWork.Save();

            return new BaseResponse()
            {
                data = result
            };
        }
    }
}