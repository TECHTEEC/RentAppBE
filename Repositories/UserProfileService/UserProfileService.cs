using Microsoft.EntityFrameworkCore;
using RentAppBE.DataContext;
using RentAppBE.Helper;
using RentAppBE.Helper.Enums;
using RentAppBE.Models;
using RentAppBE.Repositories.FilesHandleService;
using RentAppBE.Repositories.UserProfileService.Dtos.Request;
using RentAppBE.Repositories.UserProfileService.Dtos.Response;
using RentAppBE.Shared;

namespace RentAppBE.Repositories.UserProfileService
{
	public class UserProfileService(ApplicationDbContext applicationDbContext, IFilesHandleService filesHandleService) : IUserProfileService
	{
		private readonly ApplicationDbContext _db = applicationDbContext;
		private readonly IFilesHandleService _filesHandleService = filesHandleService;


		public async Task<GeneralResponse<UserProfileResponse>> GetUserProfileAsync(string userId, UserProfileRequest request)
		{
			var user = await _db.Users.FindAsync(userId);

			if (user is null)
			{
				var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid user ID").Result;

				return new GeneralResponse<UserProfileResponse>(
					false,
					request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
					null,
					StatusCodes.Status400BadRequest);
			}

			var userProfile = await _db.UserProfiles
				.Include(e => e.ApplicationUser)
				.SingleOrDefaultAsync(e => e.UserId == userId);

			if (userProfile is null)
			{
				var errorMessage = Utilities.GetErrorMessagesAsync(_db, "User does not have a profile").Result;

				return new GeneralResponse<UserProfileResponse>(
					false,
					request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
					null,
					StatusCodes.Status400BadRequest);
			}

			var userProfileResponse = new UserProfileResponse
			{
				Id = userProfile.Id,
				FullName = userProfile.FullName,
				About = userProfile.About,
				AccountType = userProfile.AccountType,
				Address = userProfile.Address,
				BankName = userProfile.BankName,
				Bio = userProfile.Bio,
				Email = userProfile.ApplicationUser.Email,
				IBAN = userProfile.IBAN,
				Lat = userProfile.Lat,
				Lon = userProfile.Lon,
				LicenseImageUrl = userProfile.LicenseImageUrl,
				LicenseNumber = userProfile.LicenseNumber,
				PhoneNumber = userProfile.ApplicationUser.PhoneNumber,
				PreferredLanguage = userProfile.PreferredLanguage,
				ProfilePhotoUrl = userProfile.ProfilePhotoUrl,
				Theme = userProfile.Theme,
				WhatsAppNumber = userProfile.WhatsAppNumber
			};

			var successMessage = Utilities.GetErrorMessagesAsync(_db, "User profile restored successfully").Result;

			return new GeneralResponse<UserProfileResponse>(
				true,
				request.lang == LangEnum.En ? successMessage.English : successMessage.Arabic,
				userProfileResponse,
				StatusCodes.Status200OK);
		}

		public async Task<GeneralResponse<AddInitialUserProfileResponse>> AddInitialUserProfileAsync(string userId, AddInitialUserProfileRequest request)
		{

			var isUserProfileExists = await _db.UserProfiles.AnyAsync(e => e.UserId == userId);

			if (isUserProfileExists)
			{
				var errorMessage = Utilities.GetErrorMessagesAsync(_db, "User already has a profile").Result;

				return new GeneralResponse<AddInitialUserProfileResponse>(
					false,
					request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
					null,
					StatusCodes.Status400BadRequest);
			}

			// validate user profile image

			if (request.ProfilePhoto is not null)
			{
				if (!filesHandleService.IsValidImageExtension(request.ProfilePhoto.FileName))
				{
					var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid image extension. Allowed: .jpg, .jpeg, .png, .gif, .bmp").Result;

					return new GeneralResponse<AddInitialUserProfileResponse>(
						false,
						request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
						null,
						StatusCodes.Status400BadRequest);
				}

				if (!filesHandleService.IsValidImageSize(request.ProfilePhoto.Length))
				{
					var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid image size. Maximum allowed is 5MB").Result;

					return new GeneralResponse<AddInitialUserProfileResponse>(
						false,
						request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
						null,
						StatusCodes.Status400BadRequest);
				}

				if (!await filesHandleService.IsValidImageContent(request.ProfilePhoto))
				{
					var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid image content").Result;

					return new GeneralResponse<AddInitialUserProfileResponse>(
						false,
						request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
						null,
						StatusCodes.Status400BadRequest);
				}
			}

			var userProfile = new UserProfile
			{
				UserId = userId,
				FullName = request.FullName,
				ProfilePhotoUrl = request.ProfilePhoto is not null ? await _filesHandleService.SaveImage(request.ProfilePhoto, "Images/UserProfile") : string.Empty,
				CreatedBy = Guid.Parse(userId),
				CreatedAt = DateTime.UtcNow
			};

			await _db.UserProfiles.AddAsync(userProfile);
			await _db.SaveChangesAsync();

			var userAddedProfileResponse = new AddInitialUserProfileResponse
			{
				FullName = userProfile.FullName,
				ProfilePhotoUrl = userProfile.ProfilePhotoUrl
			};


			var successMessage = Utilities.GetErrorMessagesAsync(_db, "User profile added successfully").Result;

			return new GeneralResponse<AddInitialUserProfileResponse>(
				true,
				request.lang == LangEnum.En ? successMessage.English : successMessage.Arabic,
				userAddedProfileResponse,
				StatusCodes.Status200OK);
		}

		public async Task<GeneralResponse<UpdateUserProfileResponse>> UpdateUserProfileAsync(string userId, UpdateUserProfileRequest request)
		{

			var isProfileExists = await _db.UserProfiles.AnyAsync(e => e.Id == request.Id);

			if (!isProfileExists)
			{
				var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid profile ID").Result;

				return new GeneralResponse<UpdateUserProfileResponse>(
					false,
					request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
					null,
					StatusCodes.Status400BadRequest);
			}

			// validate user profile image

			if (request.ProfilePhoto is not null)
			{
				if (!filesHandleService.IsValidImageExtension(request.ProfilePhoto.FileName))
				{
					var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid image extension. Allowed: .jpg, .jpeg, .png, .gif, .bmp").Result;

					return new GeneralResponse<UpdateUserProfileResponse>(
						false,
						request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
						null,
						StatusCodes.Status400BadRequest);
				}

				if (!filesHandleService.IsValidImageSize(request.ProfilePhoto.Length))
				{
					var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid image size. Maximum allowed is 5MB").Result;

					return new GeneralResponse<UpdateUserProfileResponse>(
						false,
						request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
						null,
						StatusCodes.Status400BadRequest);
				}

				if (!await filesHandleService.IsValidImageContent(request.ProfilePhoto))
				{
					var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid image content").Result;

					return new GeneralResponse<UpdateUserProfileResponse>(
						false,
						request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
						null,
						StatusCodes.Status400BadRequest);
				}
			}

			// validate whatapp number

			if (!string.IsNullOrEmpty(request.WhatsAppNumber))
			{
				if (!Utilities.IsValidWhatsAppNumber(request.WhatsAppNumber))
				{
					var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid WhatsApp Number").Result;

					return new GeneralResponse<UpdateUserProfileResponse>(
						false,
						request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
						null,
						StatusCodes.Status400BadRequest);
				}

				var isDuplicateWhatsAppNumber = await _db.UserProfiles.AnyAsync(u => u.WhatsAppNumber == request.WhatsAppNumber && u.UserId != userId);

				if (isDuplicateWhatsAppNumber)
				{
					var errorMessage = Utilities.GetErrorMessagesAsync(_db, "WhatsApp number already in use").Result;

					return new GeneralResponse<UpdateUserProfileResponse>(
						false,
						request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
						null,
						StatusCodes.Status400BadRequest);
				}
			}

			// validate IBAN
			if (!string.IsNullOrEmpty(request.IBAN) && !Utilities.IsValidIBAN(request.IBAN))
			{
				var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid IBAN number").Result;

				return new GeneralResponse<UpdateUserProfileResponse>(
					false,
					request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
					null,
					StatusCodes.Status400BadRequest);
			}

			var isDuplicateIBAN = await _db.UserProfiles.AnyAsync(u => u.IBAN == request.IBAN && u.UserId != userId);

			if (isDuplicateIBAN)
			{
				var errorMessage = Utilities.GetErrorMessagesAsync(_db, "IBAN already in use").Result;

				return new GeneralResponse<UpdateUserProfileResponse>(
					false,
					request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
					null,
					StatusCodes.Status400BadRequest);
			}

			var userProfile = new UserProfile
			{
				Id = request.Id,
				UserId = userId,
				FullName = request.FullName,
				About = request.About,
				Address = request.Address,
				BankName = request.BankName,
				Bio = request.Bio,
				IBAN = request.IBAN,
				Lat = request.Lat,
				Lon = request.Lon,
				WhatsAppNumber = request.WhatsAppNumber,
				ProfilePhotoUrl = request.ProfilePhoto is not null ? await _filesHandleService.SaveImage(request.ProfilePhoto, "Images/UserProfile") : string.Empty,
				UpdatedBy = Guid.Parse(userId),
				UpdatedAt = DateTime.UtcNow
			};

			_db.UserProfiles.Update(userProfile);
			await _db.SaveChangesAsync();

			var userUpdatedResponse = new UpdateUserProfileResponse
			{
				Id = userProfile.Id,
				UserId = Guid.Parse(userId),
				FullName = userProfile.FullName,
				About = userProfile.About,
				Address = userProfile.Address,
				BankName = userProfile.BankName,
				Bio = userProfile.Bio,
				Email = _db.Users.SingleOrDefaultAsync(e => e.Id == userId).Result.Email,
				IBAN = userProfile.IBAN,
				Lat = userProfile.Lat,
				Lon = userProfile.Lon,
				PhoneNumber = _db.Users.SingleOrDefaultAsync(e => e.Id == userId).Result.PhoneNumber,
				ProfilePhotoUrl = userProfile.ProfilePhotoUrl,
				WhatsAppNumber = userProfile.WhatsAppNumber
			};

			var successMessage = Utilities.GetErrorMessagesAsync(_db, "User profile updated successfully").Result;

			return new GeneralResponse<UpdateUserProfileResponse>(
				true,
				request.lang == LangEnum.En ? successMessage.English : successMessage.Arabic,
				userUpdatedResponse,
				StatusCodes.Status200OK);
		}
	}
}