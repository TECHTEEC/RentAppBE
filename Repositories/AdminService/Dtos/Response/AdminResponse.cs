namespace RentAppBE.Repositories.AdminService.Dtos.Response
{
	public class AdminResponse
	{
		public string Id { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string NormalizedUserName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string NormalizedEmail { get; set; } = string.Empty;
		public bool EmailConfirmed { get; set; }
		public string? PhoneNumber { get; set; }
		public bool PhoneNumberConfirmed { get; set; }
		public bool IsVerified { get; set; }
		public bool IsVendor { get; set; }
		public string? CreatedBy { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string? UpdatedBy { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public string? DeletedBy { get; set; }
		public DateTime? DeletedAt { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsActive { get; set; }
	}
}