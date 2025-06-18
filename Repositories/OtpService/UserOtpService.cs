
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentAppBE.DataContext;
using RentAppBE.Helper.Enums;
using RentAppBE.Models;
using RentAppBE.Repositories.OtpService.Dtos;
using RentAppBE.Repositories.SenderService;
using RentAppBE.Repositories.TokenService;
using RentAppBE.Repositories.TokenService.Dtos;
using RentAppBE.Shared;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RentAppBE.Repositories.OtpService
{
    public class UserOtpService : IUserOtpService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _config;
        private readonly IEmailSender _sender;
        private readonly ITokenService _tokenService;
        public UserOtpService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IConfiguration config,
            IEmailSender sender, ITokenService tokenService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _config = config;
            _sender = sender;
            _tokenService = tokenService;
        }

        public async Task<GeneralResponse<object>> SendOtpAsync(string phoneOrEmail, LangEnum lang)
        {
            if (!string.IsNullOrEmpty(phoneOrEmail) && phoneOrEmail.Contains("@"))
            {
                return await SendOtpByEmail(phoneOrEmail, lang);
            }

            return await SendOtpBySMS(phoneOrEmail, lang);

        }

        public async Task<GeneralResponse<TokenResultDto>> VerifyOtpAndRegisterAsync(string phoneOrEmail, string code, LangEnum lang, bool isVendor)
        {
            var messages = await _dbContext.UserMessages.ToListAsync();

            if (isVendor && phoneOrEmail.Contains("@"))
            {
                return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ? messages.FirstOrDefault(x => x.EnglisMsg == "You should send OTP to phone number")?.EnglisMsg :
                  messages.FirstOrDefault(x => x.EnglisMsg == "You should send OTP to phone number")?.ArabicMsg, null, 400);
            }

            else
            {


                var otp = await _dbContext.OtpRecords
                    .Where(o => o.PhoneOrEmail == phoneOrEmail && o.Code == code && !o.IsUsed && o.Expiry > DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                if (otp == null)
                {
                    return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ?
                        messages.FirstOrDefault(x => x.EnglisMsg == "Invalid or expired OTP")?.EnglisMsg :
                       messages.FirstOrDefault(x => x.EnglisMsg == "Invalid or expired OTP")?.ArabicMsg, null, 400);
                }

                otp.IsUsed = true;
                await _dbContext.SaveChangesAsync();

                var user = await _userManager.FindByEmailAsync(phoneOrEmail)
                           ?? await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneOrEmail);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = phoneOrEmail,
                        Email = phoneOrEmail.Contains("@") ? phoneOrEmail : null,
                        PhoneNumber = !phoneOrEmail.Contains("@") ? phoneOrEmail : null,
                        IsVerified = true,
                        CreatedAt = DateTime.UtcNow,
                        IsVendor = isVendor,
                        IsActive = true
                    };

                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ?
                        messages.FirstOrDefault(x => x.EnglisMsg == "Failed to create user")?.EnglisMsg :
                       messages.FirstOrDefault(x => x.EnglisMsg == "Failed to create user")?.ArabicMsg, null, 400);
                    }
                }

                return await _tokenService.CreateAccessToken(user, lang);
            }
        }


        private async Task<GeneralResponse<object>> SendOtpByEmail(string phoneOrEmail, LangEnum lang)
        {

            // Check for existing active OTP
            var activeOtp = await _dbContext.OtpRecords
                .Where(o => o.PhoneOrEmail == phoneOrEmail && !o.IsUsed && o.Expiry > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            var Messages = await _dbContext.UserMessages.ToListAsync();

            if (activeOtp != null)
            {
                if (activeOtp.ResendCount >= 2)
                {

                    return new GeneralResponse<object>(false, lang == LangEnum.En ?
                    Messages.FirstOrDefault(x => x.EnglisMsg == "Maximum OTP resend attempts reached. Please wait until it expires.")?.EnglisMsg :
                    Messages.FirstOrDefault(x => x.EnglisMsg == "Maximum OTP resend attempts reached. Please wait until it expires.")?.ArabicMsg,
                    false, 400);

                }


                // Resend the existing OTP
                activeOtp.ResendCount++;
                activeOtp.Code = new Random().Next(100000, 999999).ToString();
                activeOtp.CreatedAt = DateTime.UtcNow;
                _dbContext.OtpRecords.Update(activeOtp);
                await _dbContext.SaveChangesAsync();


                await _sender.SendEmailAsync(phoneOrEmail,
                   lang == LangEnum.En ? $"{Messages.FirstOrDefault(x => x.EnglisMsg == "OTP has been sent")?.EnglisMsg} {activeOtp.Code}" :
                   $"{Messages.FirstOrDefault(x => x.EnglisMsg == "OTP has been sent")?.ArabicMsg} {activeOtp.Code}",
                   lang == LangEnum.En ? $"{Messages.FirstOrDefault(x => x.EnglisMsg == "Your OTP is:")?.EnglisMsg} {activeOtp.Code}" :
                   $"{Messages.FirstOrDefault(x => x.EnglisMsg == "Your OTP is:")?.ArabicMsg} {activeOtp.Code}");

                return new GeneralResponse<object>(true, lang == LangEnum.En ?
                    Messages.FirstOrDefault(x => x.EnglisMsg == "OTP has been sent")?.EnglisMsg :
                    Messages.FirstOrDefault(x => x.EnglisMsg == "OTP has been sent")?.ArabicMsg, true, 200);

            }

            // No valid OTP, generate new
            var newOtpCode = new Random().Next(100000, 999999).ToString();

            var otp = new OtpRecord
            {
                PhoneOrEmail = phoneOrEmail,
                Code = newOtpCode,
                Expiry = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                ResendCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.OtpRecords.Add(otp);
            await _dbContext.SaveChangesAsync();

            await _sender.SendEmailAsync(phoneOrEmail,
                              lang == LangEnum.En ? $"{Messages.FirstOrDefault(x => x.EnglisMsg == "OTP has been sent")?.EnglisMsg} {newOtpCode}" :
                              $"{Messages.FirstOrDefault(x => x.EnglisMsg == "OTP has been sent")?.ArabicMsg} {newOtpCode}",
                              lang == LangEnum.En ? $"{Messages.FirstOrDefault(x => x.EnglisMsg == "Your OTP is:")?.EnglisMsg} {newOtpCode}" :
                              $"{Messages.FirstOrDefault(x => x.EnglisMsg == "Your OTP is:")?.ArabicMsg} {newOtpCode}");



            return new GeneralResponse<object>(true, lang == LangEnum.En ? Messages.FirstOrDefault(x => x.EnglisMsg == "OTP has been sent")?.EnglisMsg :
                Messages.FirstOrDefault(x => x.EnglisMsg == "OTP has been sent")?.ArabicMsg, true, 200);

        }

        private async Task<GeneralResponse<object>> SendOtpBySMS(string phoneOrEmail, LangEnum lang)
        {
            return new GeneralResponse<object>(true, "Test", true, 200);
        }
    }
}
