using System.ComponentModel.DataAnnotations.Schema;

namespace RentAppBE.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = default!;

        [ForeignKey("User")]
        public string UserId { get; set; } = default!;
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;


        public virtual ApplicationUser? User { get; set; }
    }
}
