using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.DTO.Login;
using Server.Application.DTO.Register;
using Server.Application.Exceptions;
using Server.Application.Features.UserAccount.Commands.Login;
using Server.Application.Features.UserAccount.Commands.Register;
using Server.Domain.Models;
using Server.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Server.Application.Features.UserAccount.Commands;
using System.Web;
using Server.Application.Utilities;

namespace Server.Infrastructure.Repositories
{
    public class UserAccount : IUserAccount
    {
        private readonly IMapper _mapper;
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;

        public UserAccount(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IMapper mapper, IEmailSender emailSender, IHttpContextAccessorWrapper httpContextAccessorWrapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
        }

        public async Task<bool> LoginUser(LoginCommand user)
        {
            await _signInManager.SignInAsync(_mapper.Map<ApplicationUser>(user), true);
            return true;
        }
        public async Task<bool> RegisterUser(RegisterCommand user)
        {
            var mappedUser = _mapper.Map<ApplicationUser>(user);
            var saveUser = await _userManager.CreateAsync(mappedUser, user.Password);
            if (!saveUser.Succeeded)
                throw new ValidationException(saveUser.Errors);
            if (user.Owner)
            {
                var currentUser = await _userManager.FindByEmailAsync(user.Email);
                var rolesResult = await _userManager.AddToRoleAsync(currentUser, UserRoles.CompanyOwner.ToString());
            }
            // register token for confirm email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(mappedUser);
            //var encodedToken = HttpUtility.UrlEncode(token);
            var dict = new Dictionary<string, string>() {
                {
                    "email",mappedUser.Email
                },
                {
                    "token",token
                }
            };
            var confirmationLink = _httpContextAccessorWrapper.ConstructUrl("api/useraccount/verifyemail", dict);
            _emailSender.SendEmailVerificationLink(mappedUser.Email, "Email verification", confirmationLink);
            return true;
        }

        public async Task<bool> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded) return true;
            else return false;
        }

        public async Task ResetPassword(ResetPasswordCommand model)
        {
            // post reset the passwor dhere

        }

        public async Task<string> GenerateResetPasswordToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // throw error!!
                var error = new ExceptionThrowHelper("Email", $"User with email: {email} was not found.");
                error.Throw();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }
    }
}