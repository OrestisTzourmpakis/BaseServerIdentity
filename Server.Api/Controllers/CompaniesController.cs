using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Contracts;
using Server.Application.Features.Companies.Commands;
using Server.Application.Features.Companies.Queries;
using Server.Application.Utilities;
using Server.Infrastructure.Persistence;

namespace Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public CompaniesController(IMediator mediator, IAuthService authService, IMapper mapper)
        {
            _mediator = mediator;
            _authService = authService;
            _mapper = mapper;
        }

        [Authorize(Roles = nameof(UserRoles.Administrator))]
        [HttpPost]
        [Route("addCompany")]
        public async Task<IActionResult> AddCompany([FromForm] AddCompanyCommand company)
        {

            return Ok(await _mediator.Send(company));
        }

        //[Authorize(Roles = nameof(Roles.Administrator))]
        [AllowAnonymous]
        [HttpGet]
        [Route("getAllCompanies")]
        public async Task<IActionResult> GetAllCompanies()
        {
            return Ok(await _mediator.Send(new GetAllCompaniesQuery()));
        }

        [Authorize(Roles = nameof(UserRoles.Administrator))]
        [HttpGet]
        [Route("getCompanyById")]
        public async Task<IActionResult> GetCompanyById(string email)
        {
            return Ok(await _mediator.Send(new GetCompanyByIdQuery() { Email = email }));
        }

        [Authorize(Roles = $"{nameof(UserRoles.Administrator)},{nameof(UserRoles.CompanyOwner)}")]
        [HttpPost]
        [Route("addUserToCompany")]
        public async Task<IActionResult> AddUserToCompany([FromBody] AddUserCompanyCommand user)
        {
            return Ok(await _mediator.Send(user));
        }

        [Authorize(Roles = nameof(UserRoles.Administrator))]
        //[AllowAnonymous]
        [HttpDelete]
        [Route("deleteCompany")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            return Ok(await _mediator.Send(new DeleteCompanyCommand() { Id = id }));
        }


        [Authorize(Roles = $"{nameof(UserRoles.Administrator)},{nameof(UserRoles.CompanyOwner)}")]
        [HttpGet]
        [Route("getCompanyByEmail")]
        public async Task<IActionResult> GetCompanyByEmail(string email)
        {
            return Ok(await _mediator.Send(new GetCompanyByEmailQuery { Email = email }));
        }

        [Authorize(Roles = nameof(UserRoles.Administrator))]
        //[AllowAnonymous]
        [HttpPut]
        [Route("updateCompany")]
        public async Task<IActionResult> UpdateCompany([FromForm] UpdateCompanyCommand company)
        {
            return Ok(await _mediator.Send(company));
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllCompaniesWithCount")]
        public async Task<IActionResult> GetAllCompaniesWithCount()
        {
            return Ok(await _mediator.Send(new GetCompaniesWithCountQuery()));
        }
    }
}