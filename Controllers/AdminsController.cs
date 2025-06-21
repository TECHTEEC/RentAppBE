using Microsoft.AspNetCore.Mvc;
using RentAppBE.Repositories.AdminService;
using RentAppBE.Repositories.AdminService.Dtos.Request;

namespace RentAppBE.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdminsController(IAdminService adminService) : ControllerBase
	{
		private readonly IAdminService _adminService = adminService;


		[HttpPost("adminUsers")]
		public async Task<IActionResult> GetAdminUsers(GetAdminFilters filters)
		{
			return Ok(await _adminService.GetAllAsync(filters));
		}

		[HttpPost("login")]
		public async Task<IActionResult> AdminLogin(AdminLoginRequest request)
		{
			return Ok(await _adminService.LoginAsync(request));
		}
	}
}