using RentAppBE.Shared;

namespace RentAppBE.Repositories.AdminService.Dtos.Request
{
	public class AdminLoginRequest : GeneralRequest
	{
		public string EmailOrUsername { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}
}
