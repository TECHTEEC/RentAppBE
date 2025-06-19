using RentAppBE.Helper.Enums;

namespace RentAppBE.Shared.Services.ValidationService
{
    public interface IValidationService
    {
        Task<(bool IsValid, string? Error)> ValidateEmail(string? email, LangEnum lang);
        Task<(bool IsValid, string? Error)> ValidatePhone(string? phone, LangEnum lang);
    }
}
