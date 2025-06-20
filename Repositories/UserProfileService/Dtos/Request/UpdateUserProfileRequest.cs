using RentAppBE.Helper.Enums;
using RentAppBE.Shared;

namespace RentAppBE.Repositories.UserProfileService.Dtos.Request
{
	public class UpdateUserProfileRequest : GeneralRequest
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string? WhatsAppNumber { get; set; } = string.Empty;
		public string? Address { get; set; } = string.Empty;
		public string? Lon { get; set; } = string.Empty;
		public string? Lat { get; set; } = string.Empty;
		public string? Bio { get; set; } = string.Empty;
		public string? About { get; set; } = string.Empty;
		public string? BankName { get; set; } = string.Empty;
		public string? IBAN { get; set; } = string.Empty;
		public IFormFile? ProfilePhoto { get; set; } = default!;
	}
}