using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Contracts;
using Server.Application.DTO.Login;
using Server.Application.DTO.Register;
using Server.Application.Features.UserAccount.Commands;
using Server.Application.Features.UserAccount.Commands.Login;
using Server.Application.Features.UserAccount.Commands.Register;
using Server.Application.Features.UserAccount.Queries;
using Server.Application.Models.Identity;
using Server.Infrastructure.Persistence;

namespace Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IEmailSender _email;

        public UserAccountController(IMediator mediator, IAuthService authService, IMapper mapper, IEmailSender email)
        {
            _mediator = mediator;
            _authService = authService;
            _mapper = mapper;
            _email = email;
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test(int id)
        {
            // var command = new UserAccountCommand();
            // var response = await _mediator.Send(command);
            if (id == 1)
            {

                _email.SendEmailAsync("otzurbakis13@gmail.com", "Hello there", "This is the content");
            }
            else if (id == 2)
            {
                _email.SendEmailVerificationLink("otzurbakis13@gmail.com", "Hello there", "This is the link");

            }
            else if (id == 3)
            {
                _email.SendEmaiForgotPassowrdLink("otzurbakis13@gmail.com", "Hello there", "This is the link");

            }
            return Ok();
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand user)
        {
            var response = await _mediator.Send(user);
            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand user)
        {
            // var command = new LoginCommand { LoginModel = user };
            // var response = await _mediator.Send(user);
            return Ok(await _authService.Login(_mapper.Map<AuthRequest>(user)));
        }



        [Authorize(Roles = nameof(Roles.Administrator))]
        [HttpGet]
        [Route("testAdmin")]
        public IActionResult Login()
        {
            return Ok("bhke boy");
        }

        [Authorize(Roles = $"{nameof(Roles.Administrator)},{nameof(Roles.CompanyOwner)}")]
        [HttpGet]
        [Route("updateUser")]
        public IActionResult AddUser()
        {
            return Ok("bhke boy");
        }
        [Authorize(Roles = $"{nameof(Roles.Administrator)}")]
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
    }
}