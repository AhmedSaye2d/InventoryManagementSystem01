using Application.DTO.Request.ActivityTracker;
using Application.DTO.Response;
using Application.DTO.Response.Activity;
using Application.Interface.ActivityTracker;
using Domain.Entities.ActivityTracker;
using Infrastructure.DataAccess;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Reposito
{
    public class ActivityTrackerRepository(AppDbContext context) : IActivityTracker
    {
        public async Task<ServicesResponse> CreateTrackerAsync(ActivityTrackerRequestDto model)
        {
            var tracker = model.Adapt<Tracker>();
            context.ActivityTrackers.Add(tracker);
            await context.SaveChangesAsync();
            return new ServicesResponse(true, "Tracker created successfully");
        }

        public async Task<IEnumerable<ActivityTrackerResponseDto>> GetTrackersAsync()
        {
            var data = await context.ActivityTrackers
                .ProjectToType<ActivityTrackerResponseDto>()
                .ToListAsync();

            return data;
        }
    }
}
