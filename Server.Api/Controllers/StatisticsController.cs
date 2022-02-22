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
    }
}