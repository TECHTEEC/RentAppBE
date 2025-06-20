using RentAppBE.Repositories.UserProfileService.Dtos.Request;
using RentAppBE.Repositories.UserProfileService.Dtos.Response;
using RentAppBE.Shared;

namespace RentAppBE.Repositories.UserProfileService
{
	public interface IUserProfileService
	{
		Task<GeneralResponse<UserProfileResponse>> GetUserProfileAsync(UserProfileRequest request);
		Task<GeneralResponse<AddInitialUserProfileResponse>> AddInitialUserProfileAsync(string userId, AddInitialUserProfileRequest request);
		Task<GeneralResponse<UpdateUserProfileResponse>> UpdateUserProfileAsync(string userId, UpdateUserProfileRequest request);
	}
}