using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.Categories.Commands;
using Server.Application.Features.Categories.Queries;

namespace Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("getCategories")]
        public async Task<IActionResult> GetCompanies()
        {
            return Ok(await _mediator.Send(new GetCategoriesQuery()));
        }

        [HttpGet]
        [Route("getCategory")]
        public async Task<IActionResult> GetCompanies(int id)
        {
            return Ok(await _mediator.Send(new GetCategoryByIdQuery { Id = id }));
        }

        [HttpDelete]
        [Route("deleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            return Ok(await _mediator.Send(new DeleteCategoryCommand { Id = id }));
        }

        [HttpPut]
        [Route("updateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryCommand model)
        {
            return Ok(await _mediator.Send(model));
        }

        [HttpPost]
        [Route("addCategory")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryCommand model)
        {
            return Ok(await _mediator.Send(model));
        }
    }
}