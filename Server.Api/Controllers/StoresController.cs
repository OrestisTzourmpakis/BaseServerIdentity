using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Contracts;
using Server.Application.Features.Stores.Commands;
using Server.Application.Features.Stores.Queries;
using Server.Infrastructure.Persistence;

namespace Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoresController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public StoresController(IMediator mediator, IAuthService authService, IMapper mapper)
        {
            _mediator = mediator;
            _authService = authService;
            _mapper = mapper;
        }

        [Authorize(Roles = nameof(Roles.Administrator))]
        [HttpPost]
        [Route("addStore")]
        public async Task<IActionResult> AddStore([FromBody] AddStoreCommand store)
        {
            return Ok(await _mediator.Send(store));
        }

        [Authorize(Roles = nameof(Roles.Administrator))]
        [HttpGet]
        [Route("getAllStores")]
        public async Task<IActionResult> GetAllStores()
        {
            return Ok(await _mediator.Send(new GetAllStoresQuery()));
        }

        [Authorize(Roles = nameof(Roles.Administrator))]
        [HttpGet]
        [Route("getStores")]
        public async Task<IActionResult> GetStores(int id)
        {
            return Ok(await _mediator.Send(new GetStoreByCompanyQuery() { CompanyId = id }));
        }

        [Authorize(Roles = nameof(Roles.Administrator))]
        [HttpDelete]
        [Route("deleteStore")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            return Ok(await _mediator.Send(new DeleteStoreCommand() { Id = id }));
        }

        [Authorize(Roles = nameof(Roles.Administrator))]
        [HttpPut]
        [Route("updateStore")]
        public async Task<IActionResult> UpdateStore([FromBody] UpdateStoreCommand store)
        {
            return Ok(await _mediator.Send(store));
        }
    }
}