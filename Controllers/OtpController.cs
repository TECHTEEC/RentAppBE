using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentAppBE.Helper.Enums;
using RentAppBE.Repositories.OtpService;

namespace RentAppBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IUserOtpService _userOtpService;

        public OtpController(IUserOtpService userOtpService)
        {
            _userOtpService = userOtpService;
        }

        [HttpGet("GetOtp")]
        public async Task<ActionResult> GetOtp(string phoneOrEmail, LangEnum lang)
        {
            return Ok(await _userOtpService.SendOtpAsync(phoneOrEmail, lang));
        }


        [HttpGet("token")]
        public async Task<ActionResult> token(string phoneOrEmail, string otp, LangEnum lang, bool isVendor)
        {
            return Ok(await _userOtpService.VerifyOtpAndRegisterAsync(phoneOrEmail, otp, lang, isVendor));
        }

    }
}
