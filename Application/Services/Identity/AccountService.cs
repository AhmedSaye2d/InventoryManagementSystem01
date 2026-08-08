using Application.DTO.Request.ActivityTracker;
using Application.DTO.Request.Identity;
using Application.DTO.Response;
using Application.DTO.Response.Activity;
using Application.Interface.Identity;

namespace Application.Services.Identity
{
    public class AccountService(IAccount account) : IAccountService
    {
        public async Task<ServicesResponse> CreateUserAsync(CreateUserRequestDto model)
        {
            return await account.CreateUserAsync(model);
        }

        public Task<IEnumerable<ActivityTrackerResponseDto>> GetActivitesAsync()
        {
            return account.GetActivityTrackerAsync();
        }

        public async Task<IEnumerable<IGrouping<DateTime, ActivityTrackerResponseDto>>> GroupActivityTracker()
        {
            var data = (await account.GetActivityTrackerAsync()).GroupBy(x => x.Date).AsEnumerable();
            return data;

        }

        public async Task<ServicesResponse> Login(LoginUserRequestDto model)
        {
            return await account.LoginAsAsync(model);
        }

        public Task SaveActivityTracker(ActivityTrackerRequestDto model)
        {
            return account.SaveActityTrackerAsync(model);
        }

        public Task SetUPAsync()
        {
            return account.SetUpAsync();
        }

        public Task<ServicesResponse> UpdateUserAsync(ChangeUserClaimRequestDto model)
        {
            return account.UpdateAsync(model);
        }


    }
}
