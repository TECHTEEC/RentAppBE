using RentAppBE.Helper.Enums;
using RentAppBE.Repositories.OtpService.Dtos;
using RentAppBE.Repositories.TokenService.Dtos;
using RentAppBE.Shared;

namespace RentAppBE.Repositories.OtpService
{
    public interface IUserOtpService
    {
        Task<GeneralResponse<object>> SendPhoneOtpAsync(string phone, LangEnum lang);
        Task<GeneralResponse<object>> SendEmailOtpAsync(string email, LangEnum lang);
        Task<GeneralResponse<TokenResultDto>> VerifyPhoneOtpAndRegisterAsync(string phone, string code, LangEnum lang, bool isVendor);
        Task<GeneralResponse<TokenResultDto>> VerifyEmailOtpAndRegisterAsync(string email, string code, LangEnum lang, bool isVendor);
        Task<GeneralResponse<TokenResultDto>> VerifyPhoneOtpAndEditAsync(string userId, string phone, string code, LangEnum lang, bool isVendor);
        Task<GeneralResponse<TokenResultDto>> VerifyEmailOtpAndEditAsync(string userId, string email, string code, LangEnum lang, bool isVendor);

    }
}
