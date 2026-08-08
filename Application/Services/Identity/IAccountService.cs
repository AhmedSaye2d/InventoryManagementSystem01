using Application.DTO.Request.ActivityTracker;
using Application.DTO.Request.Identity;
using Application.DTO.Response;
using Application.DTO.Response.Activity;

namespace Application.Services.Identity
{
    public interface IAccountService
    {
        Task<ServicesResponse> Login(LoginUserRequestDto model);
        Task<ServicesResponse> CreateUserAsync(CreateUserRequestDto model);
        Task<ServicesResponse> UpdateUserAsync(ChangeUserClaimRequestDto model);
        Task SetUPAsync();
        Task<IEnumerable<ActivityTrackerResponseDto>> GetActivitesAsync();

        Task SaveActivityTracker(ActivityTrackerRequestDto model);
        Task<IEnumerable<IGrouping<DateTime, ActivityTrackerResponseDto>>> GroupActivityTracker();
    }
}
