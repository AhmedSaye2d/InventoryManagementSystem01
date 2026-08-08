using Microsoft.AspNetCore.Identity;

namespace Application.Extention.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
