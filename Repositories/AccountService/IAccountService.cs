using RentAppBE.Helper.Enums;
using RentAppBE.Shared;

namespace RentAppBE.Repositories.AccountService
{
    public interface IAccountService
    {
        Task<GeneralResponse<bool>> DeactivateUser(string userId, LangEnum lang);
    }
}
