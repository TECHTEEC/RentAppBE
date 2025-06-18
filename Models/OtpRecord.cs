using System.ComponentModel.DataAnnotations;

namespace RentAppBE.Models
{
    public class OtpRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PhoneOrEmail { get; set; } = default!;
        public string Code { get; set; } = default!;
        public DateTime Expiry { get; set; }
        public bool IsUsed { get; set; }
        public int ResendCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
