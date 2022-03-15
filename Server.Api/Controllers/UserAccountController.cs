using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
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
    [Route("api/[controller]")]
    public class UserAccountController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IUserAccount _userAccount;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAccountController(IMediator mediator, IAuthService authService, IMapper mapper, IUserAccount userAccount, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _authService = authService;
            _mapper = mapper;
            _userAccount = userAccount;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand user)
        {
            return Ok(await _mediator.Send(user));
        }

        [HttpPost]
        [Route("registerfromadmin")]
        public async Task<IActionResult> RegisterFromAdmin([FromBody] RegisterFromAdminCommand user)
        {
            return Ok(await _mediator.Send(user));
        }



      
        [HttpPost]
        [Route("loginAdmin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginCommand user)
        {
            return Ok(await _authService.Login(_mapper.Map<AuthRequest>(user), fromAdmin: true));
        }


        [HttpPost]
        [Route("loginWebApp")]
        public async Task<IActionResult> LoginWebApp([FromBody] LoginCommand user)
        {
            return Ok(await _authService.Login(_mapper.Map<AuthRequest>(user)));
        }

        [Authorize(Roles = nameof(UserRoles.Administrator))]
        [HttpGet]
        [Route("testAdmin")]
        public IActionResult Login()
        {
            Response.Cookies.Append("jwt", "asdfas", new CookieOptions
            {
                HttpOnly = true
            });
            return Ok("bhke boy");
        }

        [Authorize]
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

        [Authorize(Roles = $"{nameof(UserRoles.Administrator)},{nameof(UserRoles.CompanyOwner)}")]
        [HttpGet]
        [Route("getUsersByCompany")]
        public async Task<IActionResult> GetUsersByCompany(string email)
        {
            return Ok(await _mediator.Send(new GetAllUsersByCompanyQuery() { Email = email }));
        }

        [Authorize(Roles = $"{nameof(UserRoles.Administrator)}")]
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
        public async Task<IActionResult> GoogleLogin([FromQuery] string viewUrl)
        {
            // return Ok(await _authService.CheckEmailConfirmation(_mapper.Map<AuthRequest>(user)));
            // get the request headers
            var con = HttpContext;
            var headers = Request.Headers.ToList();
            // Response.Headers.AccessControlAllowOrigin = "*";
            return await _mediator.Send(new GoogleLoginCommand { ViewUrl = viewUrl });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("externalLogin")]
        public async Task<IActionResult> ExternalLogin(string viewUrl, string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                // show the errors!!! from the google authentication!!
            }
            // get the login information about the user from the external login provider!!!
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // failed to load the ecternal login information!!
            }

            // if the user already has a login ( i.e if there is a record in AspNetUserLogins table  ) then sign-in the user with this external login provider
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                // return success 
            }
            else
            {
                // den yparxei o xrhsths ston table auton ara prepei na ton dhmiourghsw!!!
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    // create a new user without password if we do not have a user already
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        // create new user 
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            EmailConfirmed = true
                        };
                        await _userManager.CreateAsync(user);

                    }
                    // add a login ( i.e insert a row for the user in AspNet )
                    if(user.EmailConfirmed == false){
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                    }
                    await _userManager.AddLoginAsync(user, info);
                }
            }
            //var headers = Response.Headers.ToList();
            //var externa = new ExternalLoginInfo();
            return Redirect($"{viewUrl}/external-login?email={info.Principal.FindFirstValue(ClaimTypes.Email)}&providerKey={info.ProviderKey}&loginProvider={info.LoginProvider}");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("checkRole")]
        public async Task<IActionResult> CheckUserRole(string email)
        {
            return Ok(await _mediator.Send(new CheckUserRoleQuery() { Email = email }));
        }

        [Authorize]
        [HttpPost]
        [Route("updateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand model)
        {
            return Ok(await _mediator.Send(model));
        }
        [Authorize(Roles = $"{nameof(UserRoles.Administrator)}")]
        [HttpDelete]
        [Route("deleteUser")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            return Ok(await _mediator.Send(new DeleteUserCommand { Email = email }));
        }

        [Authorize(Roles = $"{nameof(UserRoles.Administrator)},{nameof(UserRoles.CompanyOwner)}")]
        [HttpGet]
        [Route("getUser")]
        public async Task<IActionResult> GetUser(string email)
        {
            return Ok(await _mediator.Send(new GetUserQuery { Email = email }));
        }

        [Authorize]
        [HttpPost]
        [Route("authenticateUser")]
        public async Task<IActionResult> AuthenticateUser()
        {
            return Ok(await _userAccount.AuthenticateUser());
        }

        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> LogOut()
        {
            await _userAccount.Logout();
            return Ok("user logged out!");
        }

        [Authorize]
        [HttpPost]
        [Route("sendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand model)
        {
            return Ok(await _mediator.Send(model));
        }

    }
}