using System.Security.Claims;
using Application.DTO.Request.ActivityTracker;
using Application.DTO.Request.Identity;
using Application.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // POST: api/account/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserRequestDto dto)
        {
            var response = await _accountService.Login(dto);
            if (!response.Flag)
                return BadRequest(response);
            return Ok(response);
        }


        // POST: api/account/create - Admin only
        [HttpPost("create")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto dto)
        {
            var response = await _accountService.CreateUserAsync(dto);
            if (!response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }

        // GET: api/account/users - Admin only
        [HttpGet("users")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _accountService.GetUsersAsync();
            return Ok(response);
        }

        // POST: api/account/identity/setup - Idempotent bootstrap, AllowAnonymous but safe
        [HttpPost("identity/setup")]
        [AllowAnonymous]
        public async Task<IActionResult> Setup()
        {
            await _accountService.SetUPAsync();
            return Ok("Identity setup completed successfully.");
        }

        // PUT: api/account/update-claims - Admin only
        [HttpPut("update-claims")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateUserClaims([FromBody] ChangeUserClaimRequestDto dto)
        {
            var response = await _accountService.UpdateUserAsync(dto);
            if (!response.Flag)
                return BadRequest(response.Message);
            return Ok(response);
        }

        // GET: api/account/activities
        [HttpGet("activities")]
        [Authorize(Policy = "ManagerPolicy")]
        public async Task<IActionResult> GetActivities()
        {
            var response = await _accountService.GetActivitesAsync();
            return Ok(response);
        }

        // POST: api/account/activity - UserId is taken from JWT, not from client DTO
        [HttpPost("activity")]
        [Authorize]
        public async Task<IActionResult> SaveActivity([FromBody] ActivityTrackerRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue(ClaimTypes.Email)
                      ?? "system";
            await _accountService.SaveActivityTracker(dto, userId);
            return Ok("Activity saved successfully.");
        }
    }
}
