using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Request.Identity
{
    public class CreateUserRequestDto : LoginUserRequestDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Password is required")]

        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Policy { get; set; } = string.Empty;
    }
}
