using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentAppBE.Helper.Enums;
using RentAppBE.Repositories.OtpService;
using RentAppBE.Repositories.TokenService;
using RentAppBE.Repositories.TokenService.Dtos;
using System.Security.Claims;

namespace RentAppBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IUserOtpService _userOtpService;
        private readonly ITokenService _tokenService;
        public OtpController(IUserOtpService userOtpService, ITokenService tokenService)
        {
            _userOtpService = userOtpService;
            _tokenService = tokenService;
        }

        [HttpGet("GetPhoneOtp")]
        public async Task<ActionResult> GetPhoneOtp(string phone, LangEnum lang = LangEnum.En)
        {
            return Ok(await _userOtpService.SendPhoneOtpAsync(phone, lang));
        }

        [HttpGet("GetEmailOtp")]
        public async Task<ActionResult> GetEmailOtp(string email, LangEnum lang = LangEnum.En)
        {
            return Ok(await _userOtpService.SendEmailOtpAsync(email, lang));
        }


        [HttpGet("token-phone")]
        public async Task<ActionResult> VerifyPhoneOtpAndRegisterAsync(string phone, string otp, bool isVendor, LangEnum lang = LangEnum.En)
        {
            return Ok(await _userOtpService.VerifyPhoneOtpAndRegisterAsync(phone, otp, lang, isVendor));
        }

        [HttpGet("token-email")]
        public async Task<ActionResult> VerifyEmailOtpAndRegisterAsync(string email, string otp, bool isVendor, LangEnum lang = LangEnum.En)
        {
            return Ok(await _userOtpService.VerifyEmailOtpAndRegisterAsync(email, otp, lang, isVendor));
        }


        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken(LogoutRequest input, LangEnum lang)
        {
            return Ok(await _tokenService.CreateRefreshAccessToken(input, lang));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout(LogoutRequest input, LangEnum lang)
        {
            return Ok(await _tokenService.RevokeAccessToken(input, lang));
        }


        [HttpGet("edit-phone")]
        [Authorize]
        public async Task<ActionResult> VerifyPhoneOtpAndEditAsync(string phone, string otp, bool isVendor, LangEnum lang = LangEnum.En)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(await _userOtpService.VerifyPhoneOtpAndEditAsync(userId, phone, otp, lang, isVendor));
        }

        [HttpGet("edit-email")]
        [Authorize]
        public async Task<ActionResult> VerifyEmailOtpAndEditAsync(string email, string otp, bool isVendor, LangEnum lang = LangEnum.En)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(await _userOtpService.VerifyEmailOtpAndEditAsync(userId, email, otp, lang, isVendor));
        }


        //[Authorize]
        //[HttpGet("profile")]
        //public async Task<IActionResult> GetProfile()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return BadRequest("User Id not found");
        //    }
        //    return Ok(await _tokenService.TokenProfile(userId));
        //}

    }
}
