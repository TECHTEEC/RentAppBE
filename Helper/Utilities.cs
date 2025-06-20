using Microsoft.EntityFrameworkCore;
using RentAppBE.DataContext;
using RentAppBE.Shared;
using System.Text;

namespace RentAppBE.Helper
{
	public class Utilities
	{
		public static bool IsValidWhatsAppNumber(string number)
		{
			if (string.IsNullOrWhiteSpace(number))
				return false;

			// Remove leading + and any spaces or dashes
			var cleanedNumber = number.Trim().Replace(" ", "").Replace("-", "");

			if (cleanedNumber.StartsWith("+"))
				cleanedNumber = cleanedNumber.Substring(1);

			// Check if all remaining characters are digits
			if (!cleanedNumber.All(char.IsDigit))
				return false;

			// WhatsApp typically supports numbers from 10 to 15 digits
			return cleanedNumber.Length >= 10 && cleanedNumber.Length <= 15;
		}
		public static bool IsValidIBAN(string iban)
		{
			if (string.IsNullOrWhiteSpace(iban))
				return false;

			iban = iban.Replace(" ", "").ToUpper();

			if (iban.Length < 15 || iban.Length > 34)
				return false;

			// Move the first four characters to the end
			string rearranged = iban.Substring(4) + iban.Substring(0, 4);

			// Convert letters to numbers (A=10, B=11, ..., Z=35)
			StringBuilder numericIban = new();
			foreach (char c in rearranged)
			{
				if (char.IsDigit(c))
					numericIban.Append(c);
				else if (char.IsLetter(c))
					numericIban.Append((c - 'A' + 10).ToString());
				else
					return false;
			}

			// Perform mod-97 operation
			string checkString = numericIban.ToString();
			int mod = 0;
			foreach (char digit in checkString)
			{
				mod = (mod * 10 + (digit - '0')) % 97;
			}

			return mod == 1;
		}
		public static async Task<ErrorMessage> GetErrorMessagesAsync(ApplicationDbContext db, string englishErrorMessage)
		{
			var errorMessage = new ErrorMessage();

			var message = await db.UserMessages.SingleOrDefaultAsync(e => e.EnglisMsg == englishErrorMessage);

			if (message is not null)
			{
				errorMessage.English = message.EnglisMsg;
				errorMessage.Arabic = message.ArabicMsg;
			}

			return errorMessage;
		}
	}
}