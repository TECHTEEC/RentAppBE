using RentAppBE.Repositories.AdminService.Dtos.Request;
using RentAppBE.Repositories.AdminService.Dtos.Response;
using RentAppBE.Repositories.TokenService.Dtos;
using RentAppBE.Shared;

namespace RentAppBE.Repositories.AdminService
{
	public interface IAdminService
	{
		Task<GeneralResponse<PaginatedList<AdminResponse>>> GetAllAsync(GetAdminFilters filters);
		Task<GeneralResponse<TokenResultDto>> LoginAsync(AdminLoginRequest request);
	}
}