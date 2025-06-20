namespace RentAppBE.Repositories.UserProfileService.Dtos.Response
{
	public class AddInitialUserProfileResponse
	{
		public string FullName { get; set; } = string.Empty;
		public string? ProfilePhotoUrl { get; set; } = default!;
	}
}
