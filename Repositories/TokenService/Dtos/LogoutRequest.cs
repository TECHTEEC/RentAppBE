namespace RentAppBE.Repositories.TokenService.Dtos
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = string.Empty;


        public LogoutRequest() { }

        public LogoutRequest(string refreshToken) { RefreshToken = refreshToken; }
    }
}
