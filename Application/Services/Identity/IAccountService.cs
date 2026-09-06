using Application.DTO.Request.ActivityTracker;
using Application.DTO.Request.Identity;
using Application.DTO.Response;
using Application.DTO.Response.Activity;
using Application.DTO.Response.Identity;

namespace Application.Services.Identity
{
    public interface IAccountService
    {
        Task<LoginResponseDto> Login(LoginUserRequestDto model);
        Task<ServicesResponse> CreateUserAsync(CreateUserRequestDto model);
        Task<ServicesResponse> UpdateUserAsync(ChangeUserClaimRequestDto model);
        Task SetUPAsync();
        Task<IEnumerable<GetUserWithClaimResponsDto>> GetUsersAsync();
        Task<IEnumerable<ActivityTrackerResponseDto>> GetActivitesAsync();

        Task SaveActivityTracker(ActivityTrackerRequestDto model, string authenticatedUserId);
        Task<IEnumerable<IGrouping<DateTime, ActivityTrackerResponseDto>>> GroupActivityTracker();
    }
}
