using RentAppBE.Helper.Enums;
using RentAppBE.Shared;

namespace RentAppBE.Repositories.AdminService.Dtos.Request
{
	public class GetAdminFilters : RequestFilters
	{
		public string? Email { get; set; }
		public string? UserName { get; set; }
	}
}
