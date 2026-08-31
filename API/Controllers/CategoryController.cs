using Application.DTO.Request.Product;
using Application.Services.Product.Commands.Categories;
using Application.Services.Product.Queries.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/category
        [HttpGet]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _mediator.Send(new GetAllCategoryQuery());
            return Ok(response);
        }

        // GET: api/category/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var response = await _mediator.Send(new GetCategoryByIdQuery(id));
            if (response == null) return NotFound();
            return Ok(response);
        }

        // GET: api/category/with-products
        [HttpGet("with-products")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetAllCategoriesWithProducts()
        {
            var response = await _mediator.Send(new GetAllCategoryWithProduct());
            return Ok(response);
        }

        // POST: api/category
        [HttpPost]
        [Authorize(Policy = "CreatePolicy")]
        public async Task<IActionResult> CreateCategory([FromBody] AddCategoryRequestDto dto)
        {
            var response = await _mediator.Send(new CreateCategoryCommand(dto));
            if (response != null && !response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }

        // PUT: api/category
        [HttpPut]
        [Authorize(Policy = "UpdatePolicy")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryRequestDto dto)
        {
            var response = await _mediator.Send(new UpdateCategoryCommand(dto));
            if (response != null && !response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var response = await _mediator.Send(new DeleteCategoryCommand(id));
            if (response != null && !response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }
    }
}
