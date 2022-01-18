using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.Sales.Commands;
using Server.Application.Features.Sales.Queries;

namespace Server.Api.Controllers
{
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
        public async Task<IActionResult> GetSalesUserEmail(string email)
        {
            return Ok(await _mediator.Send(new GetSalesQuery() { Email = email }));
        }

        [HttpGet]
        [Route("getAllSales")]
        public async Task<IActionResult> GetAllSales()
        {
            return Ok(await _mediator.Send(new GetAllSalesQuery()));
        }

        [HttpPut]
        [Route("updateSale")]
        public async Task<IActionResult> GetUsersByCompany([FromBody] UpdateSaleCommand model)
        {
            return Ok(await _mediator.Send(model));
        }

        [HttpPost]
        [Route("addSale")]
        public async Task<IActionResult> GetUsersByCompany([FromBody] AddSaleCommand model)
        {
            return Ok(await _mediator.Send(model));
        }

        [HttpDelete]
        [Route("deleteSale")]
        public async Task<IActionResult> GetUsersByCompany(int id)
        {
            return Ok(await _mediator.Send(new DeleteSaleCommand() { Id = id }));
        }
    }
}