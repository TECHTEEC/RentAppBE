using RentAppBE.Shared;

namespace RentAppBE.Repositories.UserProfileService.Dtos.Request
{
	public class UserProfileRequest : GeneralRequest
	{
		public string UserId { get; set; } = string.Empty;
	}
}
