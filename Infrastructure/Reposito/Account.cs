using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTO.Request.ActivityTracker;
using Application.DTO.Request.Identity;
using Application.DTO.Response;
using Application.DTO.Response.Activity;
using Application.DTO.Response.Identity;
using Application.Extention.Identity;
using Application.Interface.Identity;
using Domain.Entities.ActivityTracker;
using Infrastructure.DataAccess;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Reposito
{
    public class Account(
        UserManager<ApplicationUser> userManager,
        AppDbContext context,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration) : IAccount
    {
        public async Task<ServicesResponse> CreateUserAsync(CreateUserRequestDto model)
        {
            var user = await FindUserByEmailAsync(model.Email);
            if (user != null)
            {
                return new ServicesResponse(false, "User already exists.");
            }

            var newuser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name ?? string.Empty
            };
            var check = await userManager.CreateAsync(newuser, model.Password);
            if (!check.Succeeded)
            {
                var errors = string.Join(", ", check.Errors.Select(e => e.Description));
                return new ServicesResponse(false, string.IsNullOrWhiteSpace(errors) ? "Failed to create user." : errors);
            }
            return await CreateUserClaims(model);
        }

        private async Task<ServicesResponse> CreateUserClaims(CreateUserRequestDto model)
        {
            if (string.IsNullOrEmpty(model.Policy))
            {
                return new ServicesResponse(false, "Policy is required.");
            }

            Claim[] userclaim;
            if (model.Policy.Equals(Policy.AdminPolicy, StringComparison.OrdinalIgnoreCase))
            {
                userclaim =
                [
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, Policy.RoleClaim.Admin),
                    new Claim("Name", model.Name ?? string.Empty),
                    new Claim("create", "true"),
                    new Claim("update", "true"),
                    new Claim("delete", "true"),
                    new Claim("read", "true"),
                    new Claim("MangerUser", "true")
                ];
            }
            else if (model.Policy.Equals(Policy.ManagerPolicy, StringComparison.OrdinalIgnoreCase))
            {
                userclaim =
                [
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, Policy.RoleClaim.Manager),
                    new Claim("Name", model.Name ?? string.Empty),
                    new Claim("create", "true"),
                    new Claim("update", "true"),
                    new Claim("delete", "false"),
                    new Claim("read", "true"),
                    new Claim("MangerUser", "false")
                ];
            }
            else if (model.Policy.Equals(Policy.UserPolicy, StringComparison.OrdinalIgnoreCase))
            {
                userclaim =
                [
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, Policy.RoleClaim.User),
                    new Claim("Name", model.Name ?? string.Empty),
                    new Claim("create", "false"),
                    new Claim("update", "false"),
                    new Claim("delete", "false"),
                    new Claim("read", "true"),
                    new Claim("MangerUser", "false")
                ];
            }
            else
            {
                return new ServicesResponse(false, "Invalid policy.");
            }

            var user = await FindUserByEmailAsync(model.Email);
            if (user != null)
            {
                await userManager.AddClaimsAsync(user, userclaim);
            }
            return new ServicesResponse(true, "User created successfully.");
        }

        public async Task<IEnumerable<GetUserWithClaimResponsDto>> GetUserWithClaimAsync()
        {
            var users = userManager.Users.ToList();
            var list = new List<GetUserWithClaimResponsDto>();

            foreach (var user in users)
            {
                var claims = await userManager.GetClaimsAsync(user);
                var userDto = new GetUserWithClaimResponsDto
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email ?? string.Empty,
                    RoleName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty,
                    ManageUser = bool.TryParse(claims.FirstOrDefault(c => c.Type == "MangerUser")?.Value, out var mu) && mu,
                    Create = bool.TryParse(claims.FirstOrDefault(c => c.Type == "create")?.Value, out var c) && c,
                    Update = bool.TryParse(claims.FirstOrDefault(c => c.Type == "update")?.Value, out var u) && u,
                    Delete = bool.TryParse(claims.FirstOrDefault(c => c.Type == "delete")?.Value, out var d) && d,
                    Read = bool.TryParse(claims.FirstOrDefault(c => c.Type == "read")?.Value, out var r) && r
                };
                list.Add(userDto);
            }

            return list;
        }

        public async Task<LoginResponseDto> LoginAsAsync(LoginUserRequestDto model)
        {
            var user = await FindUserByEmailAsync(model.Email);
            if (user == null)
            {
                return new LoginResponseDto(false, Message: "User not found.");
            }
            var verify = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!verify.Succeeded)
            {
                return new LoginResponseDto(false, Message: "Invalid password.");
            }

            var claims = await userManager.GetClaimsAsync(user);
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Name ?? user.UserName ?? string.Empty)
            };
            userClaims.AddRange(claims);

            var keyString = configuration["Jwt:Key"] ?? "DefaultSuperSecretKey1234567890123456!";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"] ?? "InventoryApi",
                audience: configuration["Jwt:Audience"] ?? "InventoryClient",
                claims: userClaims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDto(true, Token: tokenString, Message: "Login successful.");
        }

        public async Task SetUpAsync()
        {
            if (!await roleManager.RoleExistsAsync(Policy.RoleClaim.Admin))
                await roleManager.CreateAsync(new IdentityRole(Policy.RoleClaim.Admin));
            if (!await roleManager.RoleExistsAsync(Policy.RoleClaim.Manager))
                await roleManager.CreateAsync(new IdentityRole(Policy.RoleClaim.Manager));
            if (!await roleManager.RoleExistsAsync(Policy.RoleClaim.User))
                await roleManager.CreateAsync(new IdentityRole(Policy.RoleClaim.User));

            var adminEmail = "admin@system.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "System Admin",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(newAdmin, "Admin@123456");
                if (result.Succeeded)
                {
                    await CreateUserClaims(new CreateUserRequestDto
                    {
                        Email = adminEmail,
                        Name = "System Admin",
                        Policy = Policy.AdminPolicy
                    });
                }
            }
        }

        public async Task<ServicesResponse> UpdateAsync(ChangeUserClaimRequestDto model)
        {
            var user = await FindUserByIdAsync(model.UserId);
            if (user == null)
            {
                return new ServicesResponse(false, "User not found.");
            }

            var existingClaims = await userManager.GetClaimsAsync(user);
            await userManager.RemoveClaimsAsync(user, existingClaims);

            var newClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, model.RoleName ?? string.Empty),
                new Claim("Name", model.Name ?? string.Empty),
                new Claim("create", model.Create.ToString().ToLower()),
                new Claim("update", model.Update.ToString().ToLower()),
                new Claim("delete", model.Delete.ToString().ToLower()),
                new Claim("read", model.Read.ToString().ToLower()),
                new Claim("MangerUser", model.ManageUser.ToString().ToLower())
            };

            var result = await userManager.AddClaimsAsync(user, newClaims);
            if (!result.Succeeded)
            {
                return new ServicesResponse(false, "Failed to update user claims.");
            }

            return new ServicesResponse(true, "User claims updated successfully.");
        }

        private async Task<ApplicationUser?> FindUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        private async Task<ApplicationUser?> FindUserByIdAsync(string id)
        {
            return await userManager.FindByIdAsync(id);
        }

        private async Task<ApplicationUser?> FindUserByIdAsync(int id)
        {
            return await userManager.FindByIdAsync(id.ToString());
        }

        public async Task SaveActityTrackerAsync(ActivityTrackerRequestDto model)
        {
            var tracker = new Tracker
            {
                Title = model.Title,
                Description = model.Description,
                UserId = model.UserId,
                OperationStatus = model.OperationStatus
            };

            context.ActivityTrackers.Add(tracker);

            await context.SaveChangesAsync();

        }

        public async Task<IEnumerable<ActivityTrackerResponseDto>> GetActivityTrackerAsync()
        {
            var list = new List<ActivityTrackerResponseDto>();

            var data = await context.ActivityTrackers
                 .ProjectToType<ActivityTrackerResponseDto>()
                 .ToListAsync();
            foreach (var activity in data)
            {
                activity.UserName = (await FindUserByIdAsync(activity.UserId))?.Name;
                list.Add(activity);
            }

            return data;
        }
    }
}
