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
using Server.Infrastructure.Helper;
using Server.Application.Models.Identity;
using FluentValidation.Results;

namespace Server.Infrastructure.Repositories
{
    public class UserAccount : IUserAccount
    {
        private readonly IMapper _mapper;
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserAccount(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IMapper mapper, IEmailSender emailSender, IHttpContextAccessorWrapper httpContextAccessorWrapper, IUnitOfWork unitOfWork)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> LoginUser(LoginCommand user)
        {
            try
            {

                await _signInManager.SignInAsync(_mapper.Map<ApplicationUser>(user), true);
                return true;
            }
            catch (Exception ex)
            {
                var failure = new ValidationFailure("User", "Incorrenct email or passsword.");
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
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

        public async Task DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            ExceptionThrowHelper error = default(ExceptionThrowHelper);
            if (user == null)
            {
                error = new ExceptionThrowHelper("Email", $"User with email: {email} was not found.");
                error.Throw();

            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                error = new ExceptionThrowHelper("Email", $"User was not deleted");
                error.Throw();
            }
        }

        public async Task<bool> RegisterUserFromAdmin(RegisterFromAdminCommand user)
        {
            var mappedUser = _mapper.Map<ApplicationUser>(user);
            user.Password = PasswordGenerator.Generate();
            var saveUser = await _userManager.CreateAsync(mappedUser, user.Password);
            if (!saveUser.Succeeded)
                throw new ValidationException(saveUser.Errors);
            if (user.Owner)
            {
                var currentUser = await _userManager.FindByEmailAsync(user.Email);
                var rolesResult = await _userManager.AddToRoleAsync(currentUser, UserRoles.CompanyOwner.ToString());
            }
            else if (!string.IsNullOrEmpty(user.CompanyOwnerEmail))
            {
                // vres to company id tou sygkekrimenou company owner
                var companyUserId = await _userManager.FindByEmailAsync(user.CompanyOwnerEmail);
                var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == companyUserId.Id);
                // add user to the company!!
                await _unitOfWork.Points.AddAsync(new Points { ApplicationUser = mappedUser, CompanyId = company.Id });
                await _unitOfWork.Save();
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
            _emailSender.SendEmailFromAdminVerificationLink(mappedUser.Email, "Email verification", confirmationLink, user.Password);
            return true;
        }

        public async Task<AuthResponse> AuthenticateUser()
        {
            var userId = _httpContextAccessorWrapper.GetHttpContext().HttpContext.User.FindFirst("uid").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            int? companyId = null;
            if (roles.Contains(UserRoles.CompanyOwner.ToString()))
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
                if (company != null) companyId = company.Id;
            }
            return new AuthResponse
            {
                Id = userId,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles,
                CompanyId = companyId
            };
        }

        public Task Logout()
        {
            _httpContextAccessorWrapper.GetHttpContext().HttpContext.Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            });

            return Task.CompletedTask;
        }
    }
}