using Microsoft.AspNetCore.Mvc;
using RentAppBE.Repositories.UserProfileService;
using RentAppBE.Repositories.UserProfileService.Dtos.Request;

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