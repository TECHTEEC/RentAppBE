using RentAppBE.Helper.Enums;
using RentAppBE.Repositories.OtpService.Dtos;

namespace RentAppBE.Repositories.OtpService
{
    public interface IUserOtpService
    {
        Task<OtpResultDto> SendOtpAsync(string phoneOrEmail, LangEnum lang);
        Task<string> VerifyOtpAndRegisterAsync(string phoneOrEmail, string code, LangEnum lang);
    }
}
