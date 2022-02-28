using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Server.Application.Features.Stores.Commands
{
    public class AddStoreCommand : IRequest<BaseResponse>
    {
        public int CompanyId { get; set; }
        public string OwnerEmail { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class AddStoreCommandHandler : IRequestHandler<AddStoreCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddStoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<BaseResponse> Handle(AddStoreCommand request, CancellationToken cancellationToken)
        {
            // if i send the owner email that means that company owner is logged in not the admin!!
            if (!string.IsNullOrEmpty(request.OwnerEmail))
            {
                // i have the user email now i want to add 
                var user = await _userManager.FindByEmailAsync(request.OwnerEmail);
                var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
                if (company == null)
                {
                    var failure = new ValidationFailure("Id", $"User with emaik: {request.OwnerEmail} does not have a registered company.");
                    throw new ValidationException(new List<ValidationFailure>() { failure });
                }
                request.CompanyId = company.Id;
            }
            var result = await _unitOfWork.Stores.AddAsync(_mapper.Map<Store>(request));
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = result
            };
        }
    }
}