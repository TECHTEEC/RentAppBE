using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentAppBE.DataContext;
using RentAppBE.Helper.Enums;
using RentAppBE.Models;
using RentAppBE.Repositories.TokenService.Dtos;
using RentAppBE.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RentAppBE.Repositories.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public TokenService(IConfiguration config,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext)
        {
            _config = config;
            _userManager = userManager;
            _dbContext = dbContext;
        }


        public async Task<GeneralResponse<TokenResultDto>> CreateAccessToken(ApplicationUser user, LangEnum lang)
        {
            var messages = await _dbContext.UserMessages.ToListAsync();

            var accessTokenExpiryMinutes = int.Parse(_config["Jwt:AccessTokenExpiryMinutes"]);
            var refreshTokenExpiryDays = int.Parse(_config["Jwt:RefreshTokenExpiryDays"]);

            // Claims
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var roles = await _userManager.GetRolesAsync(user);
            authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Access Token
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: accessTokenExpiration,
                claims: authClaims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256)
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Refresh Token
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            };
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();


            return new GeneralResponse<TokenResultDto>(true, lang == LangEnum.En ? messages?.FirstOrDefault(x => x.EnglisMsg == "The operation was completed successfully")?.EnglisMsg :
                messages?.FirstOrDefault(x => x.EnglisMsg == "The operation was completed successfully")?.ArabicMsg,
                new TokenResultDto
                {
                    AccessToken = accessToken,
                    AccessTokenExpiresIn = accessTokenExpiryMinutes * 60,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiresIn = refreshTokenExpiryDays * 24 * 60 * 60 // convert days to seconds
                }, 200);



        }

    }
}
