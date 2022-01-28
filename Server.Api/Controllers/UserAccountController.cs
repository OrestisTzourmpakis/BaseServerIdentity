using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Contracts;
using Server.Application.DTO.Login;
using Server.Application.DTO.Register;
using Server.Application.Features.UserAccount.Commands;
using Server.Application.Features.UserAccount.Commands.Login;
using Server.Application.Features.UserAccount.Commands.Register;
using Server.Application.Features.UserAccount.Queries;
using Server.Application.Models.Identity;
using Server.Application.Utilities;
using Server.Domain.Models;
using Server.Infrastructure.Persistence;

namespace Server.Api.Controllers
{
    [ApiController]
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    public class UserAccountController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IEmailSender _email;
        private readonly IUserAccount _userAccount;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserAccountController(IMediator mediator, IAuthService authService, IMapper mapper, IEmailSender email, IUserAccount userAccount, SignInManager<ApplicationUser> signInManager)
        {
            _mediator = mediator;
            _authService = authService;
            _mapper = mapper;
            _email = email;
            _userAccount = userAccount;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand user)
        {
            return Ok(await _mediator.Send(user));
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand user)
        {
            return Ok(await _authService.Login(_mapper.Map<AuthRequest>(user)));
        }

        [Authorize(Roles = nameof(UserRoles.Administrator))]
        [HttpGet]
        [Route("testAdmin")]
        public IActionResult Login()
        {
            return Ok("bhke boy");
        }

        [Authorize(Roles = $"{nameof(UserRoles.Administrator)},{nameof(UserRoles.CompanyOwner)}")]
        [HttpGet]
        [Route("updateUser")]
        public IActionResult AddUser()
        {
            return Ok("bhke boy");
        }
        [Authorize(Roles = $"{nameof(UserRoles.Administrator)}")]
        [HttpGet]
        [Route("deleteUser")]
        public IActionResult DeleteUser()
        {
            return Ok("bhke boy");
        }

        [HttpGet]
        [Route("getUsersByCompany")]
        public async Task<IActionResult> GetUsersByCompany(string email)
        {
            return Ok(await _mediator.Send(new GetAllUsersByCompanyQuery() { Email = email }));
        }

        [HttpGet]
        [Route("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _mediator.Send(new GetAllUsersQuery()));
        }

        [HttpGet]
        [Route("requestResetPassword")]
        public async Task<IActionResult> RequestResetPassword(string email)
        {
            return Ok(await _mediator.Send(new RequestResetPasswordCommand() { Email = email }));
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordModel model)
        {
            var resetPasswordModel = _mapper.Map<ResetPasswordCommand>(model);
            if (ModelState.IsValid)
                resetPasswordModel.ModelIsValid = true;
            else
                resetPasswordModel.ModelIsValid = false;
            resetPasswordModel.ViewData = ViewData;
            return await _mediator.Send(resetPasswordModel);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("resetPassword")]
        public async Task<IActionResult> ResetPasswordViewForm(string token, string email)
        {
            return await _mediator.Send(new ResetPasswordQuery() { Email = email, Token = token, ViewData = ViewData });

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("verifyemail")]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            var model = new VerifyEmailCommand()
            {
                Token = token,
                Email = email,
                ViewData = ViewData
            };
            return await _mediator.Send(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("signin-google")]
        public async Task<IActionResult> tte(string token, string email)
        {
            var model = new VerifyEmailCommand()
            {
                Token = token,
                Email = email,
                ViewData = ViewData
            };
            return await _mediator.Send(model);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("signin-google")]
        public async Task<IActionResult> ttee(string token, string email)
        {
            var model = new VerifyEmailCommand()
            {
                Token = token,
                Email = email,
                ViewData = ViewData
            };
            return await _mediator.Send(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("googleLogin")]
        public async Task<IActionResult> GoogleLogin()
        {
            // get the request headers
            var headers = Request.Headers.ToList();
            // Response.Headers.AccessControlAllowOrigin = "*";
            return await _mediator.Send(new GoogleLoginCommand());
        }

        [AllowAnonymous]
        [HttpGet]

        [Route("externalLogin")]
        public async Task<IActionResult> ExternalLogin(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var sth = await _signInManager.GetExternalLoginInfoAsync();
            var headers = Response.Headers.ToList();
            //var externa = new ExternalLoginInfo();
            return Redirect("http://localhost:3000/users");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("checkRole")]
        public async Task<IActionResult> CheckUserRole(string email)
        {
            return Ok(await _mediator.Send(new CheckUserRoleQuery() { Email = email }));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("updateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand model)
        {
            return Ok(await _mediator.Send(model));
        }

    }
}