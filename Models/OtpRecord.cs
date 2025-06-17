using System.ComponentModel.DataAnnotations;

namespace RentAppBE.Models
{
    public class OtpRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [EmailAddress]
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public bool IsUsed { get; set; } = false;
    }
}
