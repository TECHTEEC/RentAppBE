using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentAppBE.DataContext;
using RentAppBE.Helper;
using RentAppBE.Helper.Enums;
using RentAppBE.Models;
using RentAppBE.Repositories.AdminService.Dtos.Request;
using RentAppBE.Repositories.AdminService.Dtos.Response;
using RentAppBE.Repositories.TokenService;
using RentAppBE.Repositories.TokenService.Dtos;
using RentAppBE.Shared;
using System.Linq.Dynamic.Core;

namespace RentAppBE.Repositories.AdminService
{
	public class AdminService(
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager,
		ApplicationDbContext applicationDbContext,
		ITokenService tokenService) : IAdminService
	{
		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
		private readonly ApplicationDbContext _db = applicationDbContext;
		private readonly ITokenService _tokenService = tokenService;

		public async Task<GeneralResponse<PaginatedList<AdminResponse>>> GetAllAsync(GetAdminFilters filters)
		{
			var adminRole = await _db.Roles.FirstAsync(r => r.Name == "Admin");

			var query = from user in _db.Users
						join userRole in _db.UserRoles
						on user.Id equals userRole.UserId
						where userRole.RoleId == adminRole.Id
						select user;

			if (!string.IsNullOrEmpty(filters.SearchValue))
			{
				query = query.Where(e => e.Email.Trim().ToLower().Contains(filters.SearchValue.Trim().ToLower()) ||
										 e.UserName.Trim().ToLower().Contains(filters.SearchValue.Trim().ToLower()));
			}

			if (!string.IsNullOrEmpty(filters.SortColumn))
				query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");

			var admins = query
			.Select(e => new AdminResponse
			{
				Id = e.Id,
				Email = e.Email,
				NormalizedEmail = e.NormalizedEmail,
				UserName = e.UserName,
				NormalizedUserName = e.NormalizedUserName,
				PhoneNumber = e.PhoneNumber,
				PhoneNumberConfirmed = e.PhoneNumberConfirmed,
				CreatedAt = e.CreatedAt,
				CreatedBy = e.CreatedBy,
				DeletedAt = e.DeletedAt,
				DeletedBy = e.DeletedBy,
				EmailConfirmed = e.EmailConfirmed,
				IsActive = e.IsActive,
				IsDeleted = e.IsDeleted,
				IsVendor = e.IsVendor,
				IsVerified = e.IsVerified,
				UpdatedAt = e.UpdatedAt,
				UpdatedBy = e.UpdatedBy
			})
			.OrderByDescending(e => e.Id)
			.AsNoTracking();

			var adminsResult = await PaginatedList<AdminResponse>.CreateAsync(admins, filters.PageNumber, filters.PageSize);

			var successMessage = Utilities.GetErrorMessagesAsync(_db, "Admins restored successfully").Result;

			return new GeneralResponse<PaginatedList<AdminResponse>>(
										true,
										filters.lang == LangEnum.En ? successMessage.English : successMessage.Arabic,
										adminsResult,
										StatusCodes.Status200OK);
		}

		public async Task<GeneralResponse<TokenResultDto>> LoginAsync(AdminLoginRequest request)
		{
			var input = request.EmailOrUsername.Trim();

			ApplicationUser? user;

			// Check if it's an email
			if (input.Contains("@"))
			{
				if (!Utilities.IsValidEmail(input))
				{
					var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid email address").Result;

					return new GeneralResponse<TokenResultDto>(
						false,
						request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
						null,
						StatusCodes.Status400BadRequest);
				}

				user = await _userManager.FindByEmailAsync(input);

			}
			else
			{
				user = await _userManager.FindByNameAsync(input);
			}

			if (user == null)
			{
				var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid username or email").Result;

				return new GeneralResponse<TokenResultDto>(
										false,
										request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
										null,
										StatusCodes.Status400BadRequest);
			}

			if (!await _userManager.IsInRoleAsync(user, "Admin"))
			{
				var errorMessage = Utilities.GetErrorMessagesAsync(_db, "User is not an admin").Result;

				return new GeneralResponse<TokenResultDto>(
										false,
										request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
										null,
										StatusCodes.Status400BadRequest);
			}

			var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

			if (!passwordValid)
			{
				var errorMessage = Utilities.GetErrorMessagesAsync(_db, "Invalid username or password").Result;

				return new GeneralResponse<TokenResultDto>(
										false,
										request.lang == LangEnum.En ? errorMessage.English : errorMessage.Arabic,
										null,
										StatusCodes.Status400BadRequest);
			}

			var token = await _tokenService.CreateAccessToken(user, request.lang);

			var successMessage = Utilities.GetErrorMessagesAsync(_db, "Login successfully").Result;

			return new GeneralResponse<TokenResultDto>(
										true,
										request.lang == LangEnum.En ? successMessage.English : successMessage.Arabic,
										token.Data,
										StatusCodes.Status200OK);
		}
	}
}