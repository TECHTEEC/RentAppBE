using RentAppBE.Helper.Enums;

namespace RentAppBE.Repositories.TokenService.Dtos
{
    public class TokenResultDto
    {
        public string AccessToken { get; set; } = default!;
        public int AccessTokenExpiresIn { get; set; } // in seconds
        public string RefreshToken { get; set; } = default!;
        public int RefreshTokenExpiresIn { get; set; } // in seconds
        public string? UserId { get; set; } = null!;
        public string? UserName { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public bool IsVendor { get; set; }
        public VerifiedStatus? VerifiedStatus { get; set; } = null;
        public bool? IsNewUser { get; set; }
    }
}
