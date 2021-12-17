using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpatialServer.Application.Contracts;
using SpatialServer.Application.DTO.Login;
using SpatialServer.Application.DTO.Register;
using SpatialServer.Application.Features.UserAccount.Commands;
using SpatialServer.Application.Models.Identity;
using SpatialServer.Infrastructure.Persistence;

namespace SpatialServer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public UserAccountController(IMediator mediator, IAuthService authService, IMapper mapper)
        {
            _mediator = mediator;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> Test()
        {
            // var command = new UserAccountCommand();
            // var response = await _mediator.Send(command);
            return Ok();
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto user)
        {
            var command = new RegisterCommand { RegisterModel = user };
            var response = await _mediator.Send(command);
            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto user)
        {
            // var command = new LoginCommand { LoginModel = user };
            // var response = await _mediator.Send(command);
            return Ok(await _authService.Login(_mapper.Map<AuthRequest>(user)));
        }



        [Authorize(Roles = nameof(Roles.Administrator))]
        [HttpGet]
        [Route("testAdmin")]
        public async Task<IActionResult> Login()
        {
            return Ok("bhke boy");
        }
    }
}