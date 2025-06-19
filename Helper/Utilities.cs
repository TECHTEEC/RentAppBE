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
		public static bool IsValidIBAN(string number)
		{
			return true;
		}
	}
}