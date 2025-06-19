using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentAppBE.Helper.Enums;
using RentAppBE.Repositories.AccountService;
using System.Security.Claims;

namespace RentAppBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("DeactivateUser")]
        [Authorize]
        public async Task<ActionResult> DeactivateUser(LangEnum lang)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(await _accountService.DeactivateUser(userId, lang));

        }
    }
}
