using RentAppBE.Shared;

namespace RentAppBE.Repositories.UserProfileService.Dtos.Request
{
	public class AddInitialUserProfileRequest : GeneralRequest
	{
		public string FullName { get; set; } = string.Empty;
		public IFormFile? ProfilePhoto { get; set; } = default!;
	}
}
