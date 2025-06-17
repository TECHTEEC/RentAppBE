namespace RentAppBE.Repositories.OtpService.Dtos
{
    public class OtpResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public OtpResultDto() { }

        public OtpResultDto(bool success, string? message)
        {
            Success = success;
            Message = message;
        }
    }
}
