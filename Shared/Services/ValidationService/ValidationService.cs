using Microsoft.EntityFrameworkCore;
using RentAppBE.DataContext;
using RentAppBE.Helper.Enums;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RentAppBE.Shared.Services.ValidationService
{
    public class ValidationService : IValidationService
    {
        private static readonly Regex PhoneRegex = new(@"^\+9639\d{8}$");
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        private readonly ApplicationDbContext _dbContext;

        public ValidationService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(bool IsValid, string? Error)> ValidateEmail(string? email, LangEnum lang)
        {
            var messages = await _dbContext.UserMessages.ToListAsync();

            if (string.IsNullOrWhiteSpace(email))
            {
                return (false, lang == LangEnum.En ? messages?.FirstOrDefault(x => x.EnglisMsg == "Email is required")?.EnglisMsg :
                   messages?.FirstOrDefault(x => x.EnglisMsg == "Email is required")?.ArabicMsg);

            }

            if (!EmailRegex.IsMatch(email))
            {
                return (false, lang == LangEnum.En ? messages?.FirstOrDefault(x => x.EnglisMsg == "Invalid email format")?.EnglisMsg :
                  messages?.FirstOrDefault(x => x.EnglisMsg == "Invalid email format")?.ArabicMsg);
            }

            return (true, null);
        }

        public async Task<(bool IsValid, string? Error)> ValidatePhone(string? phone, LangEnum lang)
        {
            var messages = await _dbContext.UserMessages.ToListAsync();

            if (string.IsNullOrWhiteSpace(phone))
            {
                return (false, lang == LangEnum.En ? messages?.FirstOrDefault(x => x.EnglisMsg == "Phone number is required")?.EnglisMsg :
                   messages?.FirstOrDefault(x => x.EnglisMsg == "Phone number is required")?.ArabicMsg);
            }

            if (!PhoneRegex.IsMatch(phone))
            {
                return (false, lang == LangEnum.En ? messages?.FirstOrDefault(x => x.EnglisMsg == "Phone number must start with +9639 and be followed by 8 digits")?.EnglisMsg :
                  messages?.FirstOrDefault(x => x.EnglisMsg == "Phone number must start with +9639 and be followed by 8 digits")?.ArabicMsg);
            }

            return (true, null);
        }

    }
}
