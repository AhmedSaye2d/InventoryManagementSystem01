using Application.DTO.Request.ActivityTracker;
using Application.DTO.Request.Identity;
using Application.DTO.Response;
using Application.DTO.Response.Activity;
using Application.DTO.Response.Identity;

namespace Application.Interface.Identity
{
    public interface IAccount
    {
        Task<LoginResponseDto> LoginAsAsync(LoginUserRequestDto model);
        Task<ServicesResponse> CreateUserAsync(CreateUserRequestDto model);
        Task<IEnumerable<GetUserWithClaimResponsDto>> GetUserWithClaimAsync();
        Task SetUpAsync();
        Task<ServicesResponse> UpdateAsync(ChangeUserClaimRequestDto model);
        Task SaveActityTrackerAsync(ActivityTrackerRequestDto model, string authenticatedUserId);
        Task<IEnumerable<ActivityTrackerResponseDto>> GetActivityTrackerAsync();
    }
}
