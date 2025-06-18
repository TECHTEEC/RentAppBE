using Microsoft.AspNetCore.SignalR;
using RentAppBE.Helper.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentAppBE.Models
{
    public class UserProfile : AuditAggregated
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePhotoUrl { get; set; } = string.Empty;
        public string WhatsAppNumber { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string LicenseImage { get; set; } = string.Empty;
        public LangEnum PreferredLanguage { get; set; } = LangEnum.En;
        public string Address { get; set; } = string.Empty;
        public AccountTypeEnum AccountType { get; set; } = AccountTypeEnum.Individual;
        public bool IsDeleted { get; set; } = false;


        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
