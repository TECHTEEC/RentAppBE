
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentAppBE.DataContext;
using RentAppBE.Helper.Enums;
using RentAppBE.Models;
using RentAppBE.Repositories.OtpService.Dtos;
using RentAppBE.Repositories.SenderService;
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

        public UserOtpService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IConfiguration config,
            IEmailSender sender)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _config = config;
            _sender = sender;
        }



        public async Task<OtpResultDto> SendOtpAsync(string phoneOrEmail, LangEnum lang)
        {
            if (!string.IsNullOrEmpty(phoneOrEmail) && phoneOrEmail.Contains("@"))
            {
                return await SendOtpByEmail(phoneOrEmail, lang);
            }

            return await SendOtpBySMS(phoneOrEmail, lang);

        }

        public async Task<string> VerifyOtpAndRegisterAsync(string phoneOrEmail, string code, LangEnum lang)
        {
            var otp = await _dbContext.OtpRecords
                .Where(o => o.PhoneOrEmail == phoneOrEmail && o.Code == code && !o.IsUsed && o.Expiry > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (otp == null)
                throw new Exception("Invalid or expired OTP");

            otp.IsUsed = true;
            await _dbContext.SaveChangesAsync();

            var user = await _userManager.FindByEmailAsync(phoneOrEmail)
                       ?? await _userManager.FindByNameAsync(phoneOrEmail);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = phoneOrEmail,
                    Email = phoneOrEmail,
                    PhoneNumber = phoneOrEmail,
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    throw new Exception("Failed to create user");
            }

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<OtpResultDto> SendOtpByEmail(string phoneOrEmail, LangEnum lang)
        {

            // Check for existing active OTP
            var activeOtp = await _dbContext.OtpRecords
                .Where(o => o.PhoneOrEmail == phoneOrEmail && !o.IsUsed && o.Expiry > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            var resultMsg = await _dbContext.UserMessages.FirstOrDefaultAsync(x => x.EnglisMsg == "Your OTP is:");

            if (activeOtp != null)
            {
                if (activeOtp.ResendCount >= 3)
                {
                    var msg = await _dbContext.UserMessages.FirstOrDefaultAsync(x => x.EnglisMsg == "Maximum OTP resend attempts reached. Please wait until it expires.");
                    if (msg != null)
                    {
                        return new OtpResultDto(false, lang == LangEnum.En ? msg.EnglisMsg : msg.ArabicMsg);
                    }
                }


                // Resend the existing OTP
                activeOtp.ResendCount++;
                _dbContext.OtpRecords.Update(activeOtp);
                await _dbContext.SaveChangesAsync();


                await _sender.SendEmailAsync(phoneOrEmail,
                   lang == LangEnum.En ? $"{resultMsg?.EnglisMsg} {activeOtp.Code}" : $"{resultMsg?.ArabicMsg} {activeOtp.Code}",
                   lang == LangEnum.En ? $"{resultMsg?.EnglisMsg} {activeOtp.Code}" : $"{resultMsg?.ArabicMsg} {activeOtp.Code}");

                return new OtpResultDto(true, null);
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
                              lang == LangEnum.En ? $"{resultMsg?.EnglisMsg} {newOtpCode}" : $"{resultMsg?.ArabicMsg} {newOtpCode}",
                              lang == LangEnum.En ? $"{resultMsg?.EnglisMsg} {newOtpCode}" : $"{resultMsg?.ArabicMsg} {newOtpCode}");

            return new OtpResultDto(true, null);

        }

        private async Task<OtpResultDto> SendOtpBySMS(string phoneOrEmail, LangEnum lang)
        {
            return new OtpResultDto();
        }
    }
}
