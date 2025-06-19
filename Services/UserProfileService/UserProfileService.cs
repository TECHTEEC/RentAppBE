using RentAppBE.DataContext;
using RentAppBE.Models;
using RentAppBE.Services.UserProfileService.Dtos.Request;
using RentAppBE.Services.UserProfileService.Dtos.Response;
using RentAppBE.Shared;

namespace RentAppBE.Services.UserProfileService
{
	public class UserProfileService(ApplicationDbContext applicationDbContext) : IUserProfileService
	{
		private readonly ApplicationDbContext _db = applicationDbContext;

		public async Task<GeneralResponse<AddUserProfileResponse>> AddUserProfileAsync(AddUserProfileRequest request)
		{
			// validate DTO

			var userProfile = new UserProfile
			{
				UserId = request.UserId,
				FullName = request.FullName,
				WhatsAppNumber = request.WhatsAppNumber,
				ProfilePhotoUrl = request.ProfilePhotoUrl,
				AccountType = request.AccountType,
				Address = request.Address,
				Lon = request.Lon,
				Lat = request.Lat,
				Bio = request.Bio,
				About = request.About,
				BankName = request.BankName,
				IBAN = request.IBAN,
				CreatedAt = DateTime.UtcNow
			};

			await _db.UserProfiles.AddAsync(userProfile);
			await _db.SaveChangesAsync();

			var response = new AddUserProfileResponse
			{

			};

			return new GeneralResponse<AddUserProfileResponse>(true, string.Empty, response, 200);
		}
	}
}