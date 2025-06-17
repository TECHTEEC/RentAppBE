using Microsoft.AspNetCore.Identity;

namespace RentAppBE.Models
{
    public class ApplicationUser : IdentityUser
    {

        public bool IsVendor { get; set; } = false;
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public bool IsVerified { get; set; } = false;

        public virtual ICollection<UserProfile>? UserProfiles { get; set; }
    }
}
