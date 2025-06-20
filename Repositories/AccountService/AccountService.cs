using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentAppBE.DataContext;
using RentAppBE.Helper.Enums;
using RentAppBE.Models;
using RentAppBE.Repositories.OtpService;
using RentAppBE.Repositories.TokenService;
using RentAppBE.Repositories.TokenService.Dtos;
using RentAppBE.Shared;

namespace RentAppBE.Repositories.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ITokenService _tokenService;
        public AccountService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext,
            ITokenService tokenService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _tokenService = tokenService;
        }
        public async Task<GeneralResponse<bool>> DeactivateUser(string userId, LangEnum lang)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var messages = await _dbContext.UserMessages.ToListAsync();

            if (user == null)
            {
                return new GeneralResponse<bool>(false, lang == LangEnum.En ?
                    messages?.FirstOrDefault(x => x.EnglisMsg == "Invalid User")?.EnglisMsg :
                    messages?.FirstOrDefault(x => x.EnglisMsg == "Invalid User")?.ArabicMsg, false, 400);
            }

            // TODO: Check for bookings or properties before deactivation

            var guid = Guid.NewGuid().ToString();
            var utcNow = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            user.IsDeleted = true;
            user.IsActive = false;

            // Avoid duplicates by appending a unique identifier
            user.UserName += $"_{guid}({utcNow})";
            user.NormalizedUserName += $"_{guid}({utcNow})";
            user.Email += $"_{guid}({utcNow})";
            user.NormalizedEmail += $"_{guid}({utcNow})";
            user.PhoneNumber += $"_{guid}({utcNow})";
            user.DeletedBy = userId;
            user.DeletedAt = DateTime.UtcNow;

            //Revoke refresh token and logout

            var refreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == userId && t.Expires > DateTime.UtcNow && !t.IsRevoked);

            if (refreshToken != null)
            {
                await _tokenService.RevokeAccessToken(new LogoutRequest(refreshToken.Token), lang);
            }



            await _dbContext.SaveChangesAsync();

            return new GeneralResponse<bool>(true, lang == LangEnum.En ?
                    messages?.FirstOrDefault(x => x.EnglisMsg == "User Deactivated successfully")?.EnglisMsg :
                    messages?.FirstOrDefault(x => x.EnglisMsg == "User Deactivated successfully")?.ArabicMsg, true, 200);
        }
    }
}
