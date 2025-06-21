
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentAppBE.DataContext;
using RentAppBE.Helper;
using RentAppBE.Helper.Enums;
using RentAppBE.Models;
using RentAppBE.Repositories.OtpService.Dtos;
using RentAppBE.Repositories.SenderService;
using RentAppBE.Repositories.TokenService;
using RentAppBE.Repositories.TokenService.Dtos;
using RentAppBE.Shared;
using RentAppBE.Shared.Services.ValidationService;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
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
        private readonly IValidationService _validationService;

        public UserOtpService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IConfiguration config, IValidationService validationService,
            IEmailSender sender, ITokenService tokenService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _config = config;
            _sender = sender;
            _tokenService = tokenService;
            _validationService = validationService;
        }

        public async Task<GeneralResponse<object>> SendPhoneOtpAsync(string phone, LangEnum lang)
        {

            return await SendOtpBySMS(phone, lang);

        }
        public async Task<GeneralResponse<object>> SendEmailOtpAsync(string email, LangEnum lang)
        {


            return await SendOtpByEmail(email, lang);

        }
        public async Task<GeneralResponse<TokenResultDto>> VerifyPhoneOtpAndRegisterAsync(string phone, string code, LangEnum lang, bool isVendor)
        {
            var messages = await _dbContext.UserMessages.ToListAsync();
            var isPhoneValid = Utilities.IsValidWhatsAppNumber(phone);
            bool isNewRegister = false;

            if (isVendor && !isPhoneValid)
            {

                return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ? messages.FirstOrDefault(x => x.EnglisMsg == "You should send OTP to phone number")?.EnglisMsg :
                  messages.FirstOrDefault(x => x.EnglisMsg == "You should send OTP to phone number")?.ArabicMsg, null, 400);
            }

            else
            {


                var otp = await _dbContext.OtpRecords
                    .Where(o => o.PhoneOrEmail == phone && o.Code == code && !o.IsUsed && o.Expiry > DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                if (otp == null)
                {
                    return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ?
                        messages.FirstOrDefault(x => x.EnglisMsg == "Invalid or expired OTP")?.EnglisMsg :
                       messages.FirstOrDefault(x => x.EnglisMsg == "Invalid or expired OTP")?.ArabicMsg, null, 400);
                }

                otp.IsUsed = true;
                await _dbContext.SaveChangesAsync();

                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);

                if (user == null)
                {
                    isNewRegister = true;
                    user = new ApplicationUser
                    {
                        UserName = Guid.Empty.ToString() + "_" + Guid.NewGuid().ToString(),
                        Email = null,
                        EmailConfirmed = false,
                        PhoneNumber = phone,
                        PhoneNumberConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        IsVendor = isVendor,
                        IsActive = true,
                        IsVerified = false,
                    };

                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ?
                        messages.FirstOrDefault(x => x.EnglisMsg == "Failed to create user")?.EnglisMsg :
                       messages.FirstOrDefault(x => x.EnglisMsg == "Failed to create user")?.ArabicMsg, null, 400);
                    }
                }

                var auth = await _tokenService.CreateAccessToken(user, lang);
                auth.Data.UserId = user.Id;
                auth.Data.UserName = user.UserName.Contains(Guid.Empty.ToString()) ? null : user.UserName;
                auth.Data.PhoneNumber = user.PhoneNumber;
                auth.Data.Email = user.Email;
                auth.Data.VerifiedStatus = null;
                auth.Data.IsVendor = user.IsVendor;
                auth.Data.IsNewUser = isNewRegister;
                return auth;

            }
        }
        public async Task<GeneralResponse<TokenResultDto>> VerifyEmailOtpAndRegisterAsync(string email, string code, LangEnum lang, bool isVendor)
        {
            var messages = await _dbContext.UserMessages.ToListAsync();
            var (isEmailValid, emailError) = await _validationService.ValidateEmail(email, lang);
            bool isNewRegister = false;

            if (!isEmailValid)
            {

                return new GeneralResponse<TokenResultDto>(false, emailError, null, 400);
            }

            else
            {


                var otp = await _dbContext.OtpRecords
                    .Where(o => o.PhoneOrEmail == email && o.Code == code && !o.IsUsed && o.Expiry > DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                if (otp == null)
                {
                    return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ?
                        messages.FirstOrDefault(x => x.EnglisMsg == "Invalid or expired OTP")?.EnglisMsg :
                       messages.FirstOrDefault(x => x.EnglisMsg == "Invalid or expired OTP")?.ArabicMsg, null, 400);
                }

                otp.IsUsed = true;
                await _dbContext.SaveChangesAsync();

                var user = await _userManager.FindByEmailAsync(email);


                if (user == null)
                {
                    isNewRegister = true;

                    user = new ApplicationUser
                    {
                        UserName = Guid.Empty.ToString() + "_" + Guid.NewGuid().ToString(),
                        Email = email,
                        EmailConfirmed = true,
                        PhoneNumber = null,
                        PhoneNumberConfirmed = false,
                        CreatedAt = DateTime.UtcNow,
                        IsVendor = isVendor,
                        IsActive = true,
                        IsVerified = false,
                    };

                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ?
                        messages.FirstOrDefault(x => x.EnglisMsg == "Failed to create user")?.EnglisMsg :
                       messages.FirstOrDefault(x => x.EnglisMsg == "Failed to create user")?.ArabicMsg, null, 400);
                    }
                }

                var auth = await _tokenService.CreateAccessToken(user, lang);
                auth.Data.UserId = user.Id;
                auth.Data.UserName = user.UserName.Contains(Guid.Empty.ToString()) ? null : user.UserName;
                auth.Data.PhoneNumber = user.PhoneNumber;
                auth.Data.Email = user.Email;
                auth.Data.VerifiedStatus = null;
                auth.Data.IsVendor = user.IsVendor;
                auth.Data.IsNewUser = isNewRegister;
                return auth;
            }
        }
        private async Task<GeneralResponse<object>> SendOtpByEmail(string email, LangEnum lang)
        {

            var (isEmailValid, emailError) = await _validationService.ValidateEmail(email, lang);

            if (!isEmailValid)
            {
                return new GeneralResponse<object>(false, emailError, false, 400);

            }


            // Check for existing active OTP
            var activeOtp = await _dbContext.OtpRecords
                .Where(o => o.PhoneOrEmail == email && !o.IsUsed && o.Expiry > DateTime.UtcNow)
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


                await _sender.SendEmailAsync(email,
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
                PhoneOrEmail = email,
                Code = newOtpCode,
                Expiry = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                ResendCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.OtpRecords.Add(otp);
            await _dbContext.SaveChangesAsync();

            await _sender.SendEmailAsync(email,
                              lang == LangEnum.En ? $"{Messages.FirstOrDefault(x => x.EnglisMsg == "OTP has been sent")?.EnglisMsg} {newOtpCode}" :
                              $"{Messages.FirstOrDefault(x => x.EnglisMsg == "OTP has been sent")?.ArabicMsg} {newOtpCode}",
                              lang == LangEnum.En ? $"{Messages.FirstOrDefault(x => x.EnglisMsg == "Your OTP is:")?.EnglisMsg} {newOtpCode}" :
                              $"{Messages.FirstOrDefault(x => x.EnglisMsg == "Your OTP is:")?.ArabicMsg} {newOtpCode}");



            return new GeneralResponse<object>(true, lang == LangEnum.En ? Messages.FirstOrDefault(x => x.EnglisMsg == "OTP has been sent")?.EnglisMsg :
                Messages.FirstOrDefault(x => x.EnglisMsg == "OTP has been sent")?.ArabicMsg, true, 200);

        }
        private async Task<GeneralResponse<object>> SendOtpBySMS(string phone, LangEnum lang)
        {
            var isPhoneValid = Utilities.IsValidWhatsAppNumber(phone);
            var messages = await _dbContext.UserMessages.ToListAsync();

            if (!isPhoneValid)
            {
                return new GeneralResponse<object>(false, lang == LangEnum.En ?
                    messages?.FirstOrDefault(x => x.EnglisMsg == "Phone number must start with +9639 and be followed by 8 digits")?.EnglisMsg :
                  messages?.FirstOrDefault(x => x.EnglisMsg == "Phone number must start with +9639 and be followed by 8 digits")?.ArabicMsg, false, 400);

            }
            return new GeneralResponse<object>(true, "Test", true, 200);
        }
        public async Task<GeneralResponse<TokenResultDto>> VerifyPhoneOtpAndEditAsync(string userId, string phone, string code, LangEnum lang, bool isVendor)
        {
            var messages = await _dbContext.UserMessages.ToListAsync();
            var isPhoneValid = Utilities.IsValidWhatsAppNumber(phone);

            if (isVendor && !isPhoneValid)
            {

                return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ? messages.FirstOrDefault(x => x.EnglisMsg == "You should send OTP to phone number")?.EnglisMsg :
                  messages.FirstOrDefault(x => x.EnglisMsg == "You should send OTP to phone number")?.ArabicMsg, null, 400);
            }

            else
            {


                var otp = await _dbContext.OtpRecords
                    .Where(o => o.PhoneOrEmail == phone && o.Code == code && !o.IsUsed && o.Expiry > DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                if (otp == null)
                {
                    return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ?
                        messages.FirstOrDefault(x => x.EnglisMsg == "Invalid or expired OTP")?.EnglisMsg :
                       messages.FirstOrDefault(x => x.EnglisMsg == "Invalid or expired OTP")?.ArabicMsg, null, 400);
                }

                otp.IsUsed = true;
                await _dbContext.SaveChangesAsync();

                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

                if (user != null)
                {
                    user.PhoneNumber = phone;
                    user.UpdatedAt = DateTime.UtcNow;
                    user.UpdatedBy = userId;

                    var result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ?
                        messages.FirstOrDefault(x => x.EnglisMsg == "Failed to update user")?.EnglisMsg :
                       messages.FirstOrDefault(x => x.EnglisMsg == "Failed to update user")?.ArabicMsg, null, 400);
                    }
                }

                var auth = await _tokenService.CreateAccessToken(user, lang);
                auth.Data.UserId = user.Id;
                auth.Data.UserName = user.UserName.Contains(Guid.Empty.ToString()) ? null : user.UserName;
                auth.Data.PhoneNumber = user.PhoneNumber;
                auth.Data.Email = user.Email;
                auth.Data.VerifiedStatus = null;
                auth.Data.IsVendor = user.IsVendor;
                auth.Data.IsNewUser = false;
                return auth;
            }
        }
        public async Task<GeneralResponse<TokenResultDto>> VerifyEmailOtpAndEditAsync(string userId, string email, string code, LangEnum lang, bool isVendor)
        {
            var messages = await _dbContext.UserMessages.ToListAsync();
            var (isEmailValid, emailError) = await _validationService.ValidateEmail(email, lang);

            if (!isEmailValid)
            {

                return new GeneralResponse<TokenResultDto>(false, emailError, null, 400);
            }

            else
            {


                var otp = await _dbContext.OtpRecords
                    .Where(o => o.PhoneOrEmail == email && o.Code == code && !o.IsUsed && o.Expiry > DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                if (otp == null)
                {
                    return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ?
                        messages.FirstOrDefault(x => x.EnglisMsg == "Invalid or expired OTP")?.EnglisMsg :
                       messages.FirstOrDefault(x => x.EnglisMsg == "Invalid or expired OTP")?.ArabicMsg, null, 400);
                }

                otp.IsUsed = true;
                await _dbContext.SaveChangesAsync();

                var user = await _userManager.FindByIdAsync(userId);


                if (user != null)
                {
                    user.Email = email;
                    user.NormalizedEmail = email.ToUpper();
                    user.UpdatedBy = userId;
                    user.UpdatedAt = DateTime.UtcNow;
                    var result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        return new GeneralResponse<TokenResultDto>(false, lang == LangEnum.En ?
                        messages.FirstOrDefault(x => x.EnglisMsg == "Failed to update user")?.EnglisMsg :
                       messages.FirstOrDefault(x => x.EnglisMsg == "Failed to update user")?.ArabicMsg, null, 400);
                    }
                }

                var auth = await _tokenService.CreateAccessToken(user, lang);
                auth.Data.UserId = user.Id;
                auth.Data.UserName = user.UserName.Contains(Guid.Empty.ToString()) ? null : user.UserName;
                auth.Data.PhoneNumber = user.PhoneNumber;
                auth.Data.Email = user.Email;
                auth.Data.VerifiedStatus = null;
                auth.Data.IsVendor = user.IsVendor;
                auth.Data.IsNewUser = false;
                return auth;
            }
        }
    }
}