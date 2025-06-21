using RentAppBE.Helper.Enums;

namespace RentAppBE.Repositories.UserProfileService.Dtos.Response
{
	public class UpdateUserProfileResponse
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string? Email { get; set; } = string.Empty;
		public string? PhoneNumber { get; set; } = string.Empty;
		public string? WhatsAppNumber { get; set; } = string.Empty;
		public string? Address { get; set; } = string.Empty;
		public string? Lon { get; set; } = string.Empty;
		public string? Lat { get; set; } = string.Empty;
		public string? Bio { get; set; } = string.Empty;
		public string? About { get; set; } = string.Empty;
		public string? BankName { get; set; } = string.Empty;
		public string? IBAN { get; set; } = string.Empty;
		public string? ProfilePhotoUrl { get; set; } = default!;
		public string? AccountType { get; set; }
	}
}