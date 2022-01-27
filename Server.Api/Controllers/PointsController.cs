using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Contracts;
using Server.Application.Features.Points.Commands;
using Server.Application.Features.Points.Queries;
using Server.Application.Utilities;
using Server.Infrastructure.Persistence;

namespace Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PointsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public PointsController(IMediator mediator, IAuthService authService, IMapper mapper)
        {
            _mediator = mediator;
            _authService = authService;
            _mapper = mapper;
        }


        [Authorize(Roles = nameof(UserRoles.Administrator))]
        [HttpPost]
        [Route("addPoints")]
        public async Task<IActionResult> AddPoints([FromBody] AddUserPointsCommand model)
        {
            return Ok(await _mediator.Send(model));
        }

        [Authorize(Roles = nameof(UserRoles.Administrator))]
        [HttpPost]
        [Route("getPoints")]
        public IActionResult GetPoints()
        {
            return Ok("bhke boy");
        }

        [Authorize(Roles = nameof(UserRoles.Administrator))]
        [HttpPut]
        [Route("setPoints")]
        public async Task<IActionResult> SetPoints([FromBody] UpdateUserPointsCommand model)
        {
            return Ok(await _mediator.Send(model));
        }


        [HttpGet]
        [Route("getUserPointsPerCompany")]
        public async Task<IActionResult> GetUserPointsPerCompany(string email)
        {
            return Ok(await _mediator.Send(new GetUsersPointsPerCompanyQuery() { Email = email }));
        }

        [HttpGet]
        [Route("getUserPointsAllCompanies")]
        public async Task<IActionResult> GetUserPointsAllCompanies(string email)
        {
            return Ok(await _mediator.Send(new GetUserPointsAllCompaniesQuery() { Email = email }));
        }


    }
}