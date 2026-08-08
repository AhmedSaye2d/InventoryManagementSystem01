using Application.DTO.Request.ActivityTracker;
using Application.DTO.Response;
using Application.DTO.Response.Activity;

namespace Application.Interface.ActivityTracker
{
    public interface IActivityTracker
    {
        Task<ServicesResponse> CreateTrackerAsync(ActivityTrackerRequestDto model);
        Task<IEnumerable<ActivityTrackerResponseDto>> GetTrackersAsync();
    }
}
