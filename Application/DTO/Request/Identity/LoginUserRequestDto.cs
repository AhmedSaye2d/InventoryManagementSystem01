using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Request.Identity
{
    public class LoginUserRequestDto
    {
        [EmailAddress]
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Your Email is not valid")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Your password is not strong enough")]
        [MinLength(8), MaxLength(100)]
        public string Password { get; set; }
    }
}
