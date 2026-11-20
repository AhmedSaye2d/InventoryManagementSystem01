using Application.DTO.Request.Product;
using Application.Services.Product.Commands.Locations;
using Application.Services.Product.Queries.Locations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LocationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/location
        [HttpGet]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetAllLocations()
        {
            var response = await _mediator.Send(new GetAllLocationQuery());
            return Ok(response);
        }

        // GET: api/location/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetLocationById(Guid id)
        {
            var response = await _mediator.Send(new GetLocationByIdQuery(id));
            if (response == null) return NotFound();
            return Ok(response);
        }

        // POST: api/location
        [HttpPost]
        [Authorize(Policy = "CreatePolicy")]
        public async Task<IActionResult> CreateLocation([FromBody] AddLocationRequestDto dto)
        {
            var response = await _mediator.Send(new CreateLocationCommand(dto));
            if (response != null && !response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }

        // PUT: api/location
        [HttpPut]
        [Authorize(Policy = "UpdatePolicy")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationRequestDto dto)
        {
            var response = await _mediator.Send(new UpdateLocationCommand(dto));
            if (response != null && !response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }

        // DELETE: api/location/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteLocation(Guid id)
        {
            var response = await _mediator.Send(new DeleteLocationCommand(id));
            if (response != null && !response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }
    }
}
