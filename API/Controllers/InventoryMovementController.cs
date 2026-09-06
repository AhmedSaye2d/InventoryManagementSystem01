using Application.DTO.Request.Inventory;
using Application.Services.Inventory.Queries;
using Domain.Entities.Inventory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryMovementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryMovementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all inventory movements with filtering and pagination
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetInventoryMovements(
            [FromQuery] Guid? productId,
            [FromQuery] Guid? locationId,
            [FromQuery] MovementType? movementType,
            [FromQuery] string? userId,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = true)
        {
            var filter = new InventoryMovementFilterRequestDto
            {
                ProductId = productId,
                LocationId = locationId,
                MovementType = movementType,
                UserId = userId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var result = await _mediator.Send(new GetInventoryMovementsQuery(filter));
            return Ok(result);
        }

        /// <summary>
        /// Get movement by id
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetInventoryMovementByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Get history for a specific product
        /// </summary>
        [HttpGet("product/{productId:guid}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetProductHistory(
            Guid productId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            var result = await _mediator.Send(new GetProductInventoryHistoryQuery(productId, pageNumber, pageSize, dateFrom, dateTo));
            return Ok(result);
        }

        /// <summary>
        /// Stock In report (MovementType = StockIn)
        /// </summary>
        [HttpGet("reports/stock-in")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetStockInReport(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] Guid? productId = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            var result = await _mediator.Send(new GetStockInReportQuery(pageNumber, pageSize, productId, dateFrom, dateTo));
            return Ok(result);
        }

        /// <summary>
        /// Stock Out report
        /// </summary>
        [HttpGet("reports/stock-out")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetStockOutReport(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] Guid? productId = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            var result = await _mediator.Send(new GetStockOutReportQuery(pageNumber, pageSize, productId, dateFrom, dateTo));
            return Ok(result);
        }

        /// <summary>
        /// Adjustment report
        /// </summary>
        [HttpGet("reports/adjustments")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetAdjustmentReport(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] Guid? productId = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            var result = await _mediator.Send(new GetAdjustmentReportQuery(pageNumber, pageSize, productId, dateFrom, dateTo));
            return Ok(result);
        }
    }
}
