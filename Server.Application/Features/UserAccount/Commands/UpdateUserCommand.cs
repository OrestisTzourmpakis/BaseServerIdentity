using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.Exceptions;
using Server.Application.Utilities;
using Server.Domain.Models;

namespace Server.Application.Features.UserAccount.Commands
{
    public class UpdateUserCommand : IRequest<Unit>
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool Owner { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            var sendConfirmationEmail = false;
            if (!user.Email.Equals(request.Email.Trim()))
            {
                // if the email has been changed then we have to send the email
                user.Email = request.Email;
                sendConfirmationEmail = true;
            }
            if (!user.UserName.Equals(request.UserName.Trim()))
                user.UserName = request.UserName;
            // save the user first and then check his role!!
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new ValidationException(result.Errors);
            }
            var isOwner = await _userManager.IsInRoleAsync(user, UserRoles.CompanyOwner.ToString());
            if (request.Owner != isOwner)
            {
                // check if the user is a company owner!!
                if (request.Owner)
                    await _userManager.AddToRoleAsync(user, UserRoles.CompanyOwner.ToString());
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, UserRoles.CompanyOwner.ToString());
                    // check if the user had a company!!
                    var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
                    if (company != null)
                    {
                        company.ApplicationUserId = null;
                        await _unitOfWork.Companies.UpdateAsync(company);
                        await _unitOfWork.Save();
                    }

                }
            }
            return Unit.Value;
        }
    }
}