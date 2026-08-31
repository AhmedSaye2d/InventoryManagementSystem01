using Application.DTO.Request.Product;
using Application.Services.Product.Commands.Products;
using Application.Services.Product.Queries.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/product
        [HttpGet]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _mediator.Send(new GetProductQuery());
            return Ok(response);
        }

        // GET: api/product/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var response = await _mediator.Send(new GetProductByIdQuery(id));
            if (response == null) return NotFound();
            return Ok(response);
        }

        // POST: api/product
        [HttpPost]
        [Authorize(Policy = "CreatePolicy")]
        public async Task<IActionResult> CreateProduct([FromBody] AddProductRequestDto dto)
        {
            var response = await _mediator.Send(new CreateProductCommand(dto));
            if (response != null && !response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }

        // PUT: api/product/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "UpdatePolicy")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequestDto dto)
        {
            var response = await _mediator.Send(new UpdateProductCommand(id, dto));
            if (response != null && !response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var response = await _mediator.Send(new DeleteProductCommand(id));
            if (response != null && !response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }
    }
}
