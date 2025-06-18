namespace RentAppBE.Models
{
    public class UserMessage
    {
        public Guid Id { get; set; }
        public string ArabicMsg { get; set; } = string.Empty;
        public string EnglisMsg { get; set; } = string.Empty;

    }
}
