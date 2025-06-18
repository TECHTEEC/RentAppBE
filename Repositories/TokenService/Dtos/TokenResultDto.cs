namespace RentAppBE.Repositories.TokenService.Dtos
{
    public class TokenResultDto
    {
        public string AccessToken { get; set; } = default!;
        public int AccessTokenExpiresIn { get; set; } // in seconds
        public string RefreshToken { get; set; } = default!;
        public int RefreshTokenExpiresIn { get; set; } // in seconds
    }
}
