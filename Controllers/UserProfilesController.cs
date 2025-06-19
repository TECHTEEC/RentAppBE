using Microsoft.AspNetCore.Mvc;
using RentAppBE.Services.UserProfileService;
using RentAppBE.Services.UserProfileService.Dtos.Request;

namespace RentAppBE.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserProfilesController(IUserProfileService userProfileService) : ControllerBase
	{
		private readonly IUserProfileService _userProfileService = userProfileService;


		[HttpPost]
		public async Task<IActionResult> Add(AddUserProfileRequest request)
		{
			return Ok(await _userProfileService.AddUserProfileAsync(request));
		}
	}
}