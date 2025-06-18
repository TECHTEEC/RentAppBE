using RentAppBE.Helper.Enums;
using RentAppBE.Repositories.OtpService.Dtos;
using RentAppBE.Repositories.TokenService.Dtos;
using RentAppBE.Shared;

namespace RentAppBE.Repositories.OtpService
{
    public interface IUserOtpService
    {
        Task<GeneralResponse<object>> SendOtpAsync(string phoneOrEmail, LangEnum lang);
        Task<GeneralResponse<TokenResultDto>> VerifyOtpAndRegisterAsync(string phoneOrEmail, string code, LangEnum lang, bool isVendor);
    }
}
