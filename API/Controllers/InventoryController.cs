using System.Security.Claims;
using Application.DTO.Request.Inventory;
using Application.Services.Inventory.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue("sub")
                ?? "system";
        }

        /// <summary>
        /// Stock In: increase product quantity + create StockIn movement
        /// </summary>
        [HttpPost("stock-in")]
        [Authorize(Policy = "CreatePolicy")]
        public async Task<IActionResult> StockIn([FromBody] StockInRequestDto dto)
        {
            var userId = GetUserId();
            var result = await _mediator.Send(new StockInCommand(dto, userId));
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Stock Out: decrease product quantity + create StockOut movement
        /// </summary>
        [HttpPost("stock-out")]
        [Authorize(Policy = "CreatePolicy")]
        public async Task<IActionResult> StockOut([FromBody] StockOutRequestDto dto)
        {
            var userId = GetUserId();
            var result = await _mediator.Send(new StockOutCommand(dto, userId));
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
