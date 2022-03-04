using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.Statistics.Query;

namespace Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private IMediator _mediator;

        public StatisticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("getTotalUsers")]
        [HttpPost]
        public async Task<IActionResult> GetTotalUsers([FromBody] GetTotalUsersQuery model)
        {
            return Ok(await _mediator.Send(model));
        }

        [Route("getTotalStores")]
        [HttpPost]
        public async Task<IActionResult> GetTotalStores([FromBody] GetTotalStoresQuery model)
        {
            return Ok(await _mediator.Send(model));
        }

        [Route("getTotalActiveSales")]
        [HttpPost]
        public async Task<IActionResult> GetTotalActiveSales([FromBody] GetTotalActiveSalesQuery model)
        {
            return Ok(await _mediator.Send(model));
        }

        [Route("getLast6Months")]
        [HttpPost]
        public async Task<IActionResult> GetLast6Months([FromBody] GetLast6MonthsPointsQuery model)
        {
            return Ok(await _mediator.Send(model));
        }

        [Route("getTotalEarnedAndRedeemPoint")]
        [HttpPost]
        public async Task<IActionResult> GetTotalEarnedAndRedeemPoint([FromBody] GetTotalEarnedAndRedeemPointsQuery model)
        {
            return Ok(await _mediator.Send(model));
        }

        [Route("getTop30Users")]
        [HttpPost]
        public async Task<IActionResult> GetTop30Users([FromBody] GetTop30UsersQuery model)
        {
            return Ok(await _mediator.Send(model));
        }

        [Route("getTotalCompanies")]
        [HttpGet]
        public async Task<IActionResult> GetTotalCompanies()
        {
            return Ok(await _mediator.Send(new GetTotalCompaniesQuery()));
        }
    }
}