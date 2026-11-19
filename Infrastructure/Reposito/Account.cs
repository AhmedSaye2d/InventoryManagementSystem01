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
        // Allowed roles for normal user creation (Admin creation only via Setup)
        private static readonly HashSet<string> AllowedCreateRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            Policy.RoleClaim.Manager,
            Policy.RoleClaim.User
        };

        private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            Policy.RoleClaim.Admin,
            Policy.RoleClaim.Manager,
            Policy.RoleClaim.User
        };

        public async Task<ServicesResponse> CreateUserAsync(CreateUserRequestDto model)
        {
            // Prevent privilege escalation: only Manager/User allowed via this endpoint
            // Do NOT trust Policy from client; use Role field with strict allow-list
            if (string.IsNullOrWhiteSpace(model.Role) || !AllowedCreateRoles.Contains(model.Role))
            {
                return new ServicesResponse(false, "Invalid role. Allowed values: Manager, User. Admin creation is only via Setup.");
            }

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
            if (string.IsNullOrEmpty(model.Role))
            {
                return new ServicesResponse(false, "Role is required.");
            }

            // Double-check: block Admin here (defense in depth)
            if (model.Role.Equals(Policy.RoleClaim.Admin, StringComparison.OrdinalIgnoreCase))
            {
                return new ServicesResponse(false, "Creating Admin via this endpoint is not allowed.");
            }

            if (!AllowedCreateRoles.Contains(model.Role))
            {
                return new ServicesResponse(false, "Invalid role. Allowed: Manager, User.");
            }

            string roleName;
            Claim[] permissionClaims;

            if (model.Role.Equals(Policy.RoleClaim.Manager, StringComparison.OrdinalIgnoreCase))
            {
                roleName = Policy.RoleClaim.Manager;
                permissionClaims =
                [
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim("Name", model.Name ?? string.Empty),
                    new Claim("create", "true"),
                    new Claim("update", "true"),
                    new Claim("delete", "false"),
                    new Claim("read", "true"),
                    new Claim("MangerUser", "false")
                ];
            }
            else if (model.Role.Equals(Policy.RoleClaim.User, StringComparison.OrdinalIgnoreCase))
            {
                roleName = Policy.RoleClaim.User;
                permissionClaims =
                [
                    new Claim(ClaimTypes.Email, model.Email),
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
                return new ServicesResponse(false, "Invalid role.");
            }

            var user = await FindUserByEmailAsync(model.Email);
            if (user != null)
            {
                // Assign Identity Role (for RequireRole checks) + permission claims
                var roleResult = await userManager.AddToRoleAsync(user, roleName);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return new ServicesResponse(false, $"Failed to assign role: {errors}");
                }
                // Also add role as claim for JWT compatibility (some policies use ClaimTypes.Role)
                var allClaims = permissionClaims.Append(new Claim(ClaimTypes.Role, roleName)).ToArray();
                await userManager.AddClaimsAsync(user, allClaims);
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
                var roles = await userManager.GetRolesAsync(user);
                // Prefer Identity Role store, fallback to claim
                var roleFromStore = roles.FirstOrDefault();
                var roleFromClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var effectiveRole = !string.IsNullOrEmpty(roleFromStore) ? roleFromStore : roleFromClaim ?? string.Empty;

                var userDto = new GetUserWithClaimResponsDto
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email ?? string.Empty,
                    RoleName = effectiveRole,
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
            var roles = await userManager.GetRolesAsync(user);

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Name ?? user.UserName ?? string.Empty)
            };
            userClaims.AddRange(claims);
            // Add roles as ClaimTypes.Role for JWT + policies
            foreach (var role in roles)
            {
                if (!userClaims.Any(c => c.Type == ClaimTypes.Role && c.Value == role))
                    userClaims.Add(new Claim(ClaimTypes.Role, role));
            }

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
            // Idempotent: create roles if not exist
            if (!await roleManager.RoleExistsAsync(Policy.RoleClaim.Admin))
                await roleManager.CreateAsync(new IdentityRole(Policy.RoleClaim.Admin));
            if (!await roleManager.RoleExistsAsync(Policy.RoleClaim.Manager))
                await roleManager.CreateAsync(new IdentityRole(Policy.RoleClaim.Manager));
            if (!await roleManager.RoleExistsAsync(Policy.RoleClaim.User))
                await roleManager.CreateAsync(new IdentityRole(Policy.RoleClaim.User));

            // Seed admin credentials from configuration (env vars) with safe fallback for dev
            var adminEmail = configuration["SeedAdmin:Email"] ?? "admin@system.com";
            var adminPassword = configuration["SeedAdmin:Password"] ?? "Admin@123456";
            var adminName = configuration["SeedAdmin:Name"] ?? "System Admin";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = adminName,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                {
                    var createdAdmin = await userManager.FindByEmailAsync(adminEmail);
                    if (createdAdmin != null)
                    {
                        // Assign Admin role via Identity
                        await userManager.AddToRoleAsync(createdAdmin, Policy.RoleClaim.Admin);
                        var adminClaims = new[]
                        {
                            new Claim(ClaimTypes.Email, adminEmail),
                            new Claim(ClaimTypes.Role, Policy.RoleClaim.Admin),
                            new Claim("Name", adminName),
                            new Claim("create", "true"),
                            new Claim("update", "true"),
                            new Claim("delete", "true"),
                            new Claim("read", "true"),
                            new Claim("MangerUser", "true")
                        };
                        await userManager.AddClaimsAsync(createdAdmin, adminClaims);
                    }
                }
            }
            // If admin already exists, do nothing -> idempotent, no duplication, no overwrite
        }

        public async Task<ServicesResponse> UpdateAsync(ChangeUserClaimRequestDto model)
        {
            var user = await FindUserByIdAsync(model.UserId);
            if (user == null)
            {
                return new ServicesResponse(false, "User not found.");
            }

            if (string.IsNullOrWhiteSpace(model.RoleName) || !AllowedRoles.Contains(model.RoleName))
            {
                return new ServicesResponse(false, "Invalid RoleName. Allowed: Admin, Manager, User.");
            }

            // Prevent privilege escalation via arbitrary claims: validate RoleName matches permission set?
            // Only Admin can call this (enforced by controller), so allow any valid role.
            var existingClaims = await userManager.GetClaimsAsync(user);
            await userManager.RemoveClaimsAsync(user, existingClaims);

            // Update Identity Roles
            var currentRoles = await userManager.GetRolesAsync(user);
            if (currentRoles.Any())
                await userManager.RemoveFromRolesAsync(user, currentRoles);
            var roleAddResult = await userManager.AddToRoleAsync(user, model.RoleName);
            if (!roleAddResult.Succeeded)
            {
                return new ServicesResponse(false, "Failed to update role.");
            }

            var newClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, model.RoleName),
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

        public async Task SaveActityTrackerAsync(ActivityTrackerRequestDto model, string authenticatedUserId)
        {
            // Never trust UserId from client DTO - use authenticated user from JWT
            var tracker = new Tracker
            {
                Title = model.Title,
                Description = model.Description,
                UserId = authenticatedUserId,
                Operation = model.Operation,
                Date = DateTime.UtcNow,
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
