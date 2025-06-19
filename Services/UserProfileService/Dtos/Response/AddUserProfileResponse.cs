using RentAppBE.Helper.Enums;

namespace RentAppBE.Services.UserProfileService.Dtos.Response
{
	public class AddUserProfileResponse
	{
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string? WhatsAppNumber { get; set; } = string.Empty;
		public string? ProfilePhotoUrl { get; set; } = string.Empty; //UpdateLater
		public AccountTypeEnum AccountType { get; set; } = AccountTypeEnum.Individual;
		public string? Address { get; set; } = string.Empty;
		public string? Lon { get; set; } = string.Empty;
		public string? Lat { get; set; } = string.Empty;
		public string? Bio { get; set; } = string.Empty;
		public string? About { get; set; } = string.Empty;
		public string? BankName { get; set; } = string.Empty;
		public string? IBAN { get; set; } = string.Empty;
	}
}