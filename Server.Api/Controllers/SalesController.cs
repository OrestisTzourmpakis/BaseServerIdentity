using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.Sales.Commands;
using Server.Application.Features.Sales.Queries;
using Server.Application.Utilities;

namespace Server.Api.Controllers
{
    //[Authorize(Roles = $"{nameof(UserRoles.Administrator)},{nameof(UserRoles.CompanyOwner)}")]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public SalesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("getSales")]
        public async Task<IActionResult> GetSalesUserEmail(string email, int id)
        {
            return Ok(await _mediator.Send(new GetSalesQuery() { Email = email, Id = id }));
        }


        [HttpGet]
        [Route("getActiveSalesByCompanyId")]
        public async Task<IActionResult> GetActiveSalesByCompanyId(int id)
        {
            return Ok(await _mediator.Send(new GetActiveSalesByCompanyIdQuery { Id = id }));
        }

        [HttpGet]
        [Route("getAllActiveSales")]
        public async Task<IActionResult> GetAllActiveSales()
        {
            return Ok(await _mediator.Send(new GetAllActiveSalesQuery()));
        }

        [HttpGet]
        [Route("getAllSales")]
        public async Task<IActionResult> GetAllSales()
        {
            return Ok(await _mediator.Send(new GetAllSalesQuery()));
        }
        [Authorize(Roles = $"{nameof(UserRoles.Administrator)},{nameof(UserRoles.CompanyOwner)}")]
        [HttpPut]
        [Route("updateSale")]
        public async Task<IActionResult> GetUsersByCompany([FromForm] UpdateSaleCommand model)
        {
            return Ok(await _mediator.Send(model));
        }
        [Authorize(Roles = $"{nameof(UserRoles.Administrator)},{nameof(UserRoles.CompanyOwner)}")]
        [HttpPost]
        [Route("addSale")]
        public async Task<IActionResult> AddSale([FromForm] AddSaleCommand model)
        {
            return Ok(await _mediator.Send(model));
        }
        [Authorize(Roles = $"{nameof(UserRoles.Administrator)},{nameof(UserRoles.CompanyOwner)}")]
        [HttpDelete]
        [Route("deleteSale")]
        public async Task<IActionResult> GetUsersByCompany(int id)
        {
            return Ok(await _mediator.Send(new DeleteSaleCommand() { Id = id }));
        }
    }
}