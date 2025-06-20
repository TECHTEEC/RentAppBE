using RentAppBE.Helper.Enums;

namespace RentAppBE.Repositories.UserProfileService.Dtos.Response
{
	public class UserProfileResponse
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string? WhatsAppNumber { get; set; } = string.Empty;
		public string? ProfilePhotoUrl { get; set; } = string.Empty;
		public AccountTypeEnum AccountType { get; set; } = AccountTypeEnum.Individual;
		public string? Address { get; set; } = string.Empty;
		public string? Lon { get; set; } = string.Empty;
		public string? Lat { get; set; } = string.Empty;
		public string? Bio { get; set; } = string.Empty;
		public string? About { get; set; } = string.Empty;
		public string? BankName { get; set; } = string.Empty;
		public string? IBAN { get; set; } = string.Empty;
		public string? LicenseNumber { get; set; } = string.Empty;
		public string? LicenseImageUrl { get; set; } = string.Empty;
		public LangEnum PreferredLanguage { get; set; } = LangEnum.En;
		public ThemeEnum Theme { get; set; } = ThemeEnum.Light;

	}
}
