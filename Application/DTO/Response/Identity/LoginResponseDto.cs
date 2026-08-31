namespace Application.DTO.Response.Identity
{
    public record LoginResponseDto(bool Flag, string Token = null!, string RefreshToken = null!, string Message = null!);
}
