﻿using System.Security.Claims;

namespace RentAppBE.Extensions
{
	public static class UserExtensions
	{
		public static string? GetUserId(this ClaimsPrincipal user)
		{
			return user.FindFirstValue(ClaimTypes.NameIdentifier);
		}
	}
}