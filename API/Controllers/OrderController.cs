using Application.DTO.Request.Order;
using Application.Services.Order.Commands;
using Application.Services.Order.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/order
        [HttpGet]
        [Authorize(Policy = "ManagerPolicy")]
        public async Task<IActionResult> GetAllOrders()
        {
            var response = await _mediator.Send(new GetAllOrderQuery());
            return Ok(response);
        }

        // GET: api/order/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            var response = await _mediator.Send(new GetOrderById(userId));
            return Ok(response);
        }

        // GET: api/order/count
        [HttpGet("count")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetOrderCount([FromQuery] string userId = null, [FromQuery] bool isAdmin = false)
        {
            var response = await _mediator.Send(new GetGenericOrderCountQuery(userId, isAdmin));
            return Ok(response);
        }

        // GET: api/order/monthly
        [HttpGet("monthly")]
        [Authorize(Policy = "ManagerPolicy")]
        public async Task<IActionResult> GetProductOrderByMonth([FromQuery] string userId = null)
        {
            var response = await _mediator.Send(new GetProductOrderByMonthQuery(userId));
            return Ok(response);
        }

        // GET: api/order/top-ordered
        [HttpGet("top-ordered")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetTopOrderedProduct([FromQuery] string userId = null)
        {
            var response = await _mediator.Send(new GetOrderPorductWithQuantityQuery(userId));
            return Ok(response);
        }

        // POST: api/order
        [HttpPost]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto dto)
        {
            var response = await _mediator.Send(new CreateOrderCommand(dto));
            if (response != null && !response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }

        // PUT: api/order
        [HttpPut]
        [Authorize(Policy = "ManagerPolicy")]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateClientOrderRequestDto dto)
        {
            var response = await _mediator.Send(new UpdateOrderCommand(dto));
            if (response != null && !response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }

        // PUT: api/order/cancel/{orderId:guid}
        [HttpPut("cancel/{orderId:guid}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            var response = await _mediator.Send(new CancelOrderCommand(orderId));
            if (response != null && !response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }
    }
}
