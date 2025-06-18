using RentAppBE.Helper.Enums;
using RentAppBE.Models;
using RentAppBE.Repositories.TokenService.Dtos;
using RentAppBE.Shared;

namespace RentAppBE.Repositories.TokenService
{
    public interface ITokenService
    {
        Task<GeneralResponse<TokenResultDto>> CreateAccessToken(ApplicationUser user, LangEnum lang);
    }
}
