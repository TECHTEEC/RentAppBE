using RentAppBE.Helper.Enums;
using RentAppBE.Repositories.UserProfileService.Dtos.Request;
using RentAppBE.Repositories.UserProfileService.Dtos.Response;
using RentAppBE.Shared;

namespace RentAppBE.Repositories.UserProfileService
{
	public interface IUserProfileService
	{
		Task<GeneralResponse<AddUserProfileResponse>> AddUserProfileAsync(AddUserProfileRequest request);
	}
}