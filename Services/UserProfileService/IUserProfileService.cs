using RentAppBE.Services.UserProfileService.Dtos.Request;
using RentAppBE.Services.UserProfileService.Dtos.Response;
using RentAppBE.Shared;

namespace RentAppBE.Services.UserProfileService
{
	public interface IUserProfileService
	{
		Task<GeneralResponse<AddUserProfileResponse>> AddUserProfileAsync(AddUserProfileRequest request);
	}
}