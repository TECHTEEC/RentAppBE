using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentAppBE.Extensions;
using RentAppBE.Repositories.UserProfileService;
using RentAppBE.Repositories.UserProfileService.Dtos.Request;

namespace RentAppBE.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UserProfilesController(IUserProfileService userProfileService) : ControllerBase
	{
		private readonly IUserProfileService _userProfileService = userProfileService;


		[HttpPost("GetUserProfile")]
		public async Task<IActionResult> GetUserProfile(UserProfileRequest request)
		{
			var userId = User.GetUserId()!;

			return Ok(await _userProfileService.GetUserProfileAsync(userId, request));
		}


		[HttpPost("AddInitialUserProfile")]
		public async Task<IActionResult> AddInitialUserProfile([FromForm] AddInitialUserProfileRequest request)
		{
			var userId = User.GetUserId()!;

			return Ok(await _userProfileService.AddInitialUserProfileAsync(userId, request));
		}


		[HttpPut("UpdateUserProfile")]
		public async Task<IActionResult> UpdateUserProfile([FromForm] UpdateUserProfileRequest request)
		{
			var userId = User.GetUserId()!;

			return Ok(await _userProfileService.UpdateUserProfileAsync(userId, request));
		}
	}
}