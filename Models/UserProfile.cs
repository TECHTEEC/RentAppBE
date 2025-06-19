using RentAppBE.Helper.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentAppBE.Models
{
	public class UserProfile : AuditAggregated
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string FullName { get; set; } = string.Empty;
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


		[ForeignKey("ApplicationUser")]
		public string UserId { get; set; } = string.Empty;
		public virtual ApplicationUser? ApplicationUser { get; set; }
	}
}