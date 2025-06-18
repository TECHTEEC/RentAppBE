namespace RentAppBE.Repositories.TokenService.Dtos
{
    public class RefreshTokenDto
    {
        public int Id { get; set; }
        public string Token { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;

        public RefreshTokenDto() { }
        public RefreshTokenDto(int id, string token, string userId, DateTime expires,
            bool isRevoked, bool isUsed)
        {
            Id = id;
            Token = token;
            UserId = userId;
            Expires = expires;
            IsRevoked = isRevoked;
            IsUsed = isUsed;
        }
    }
}
