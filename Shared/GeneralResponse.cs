namespace RentAppBE.Shared
{
    public class GeneralResponse<T>
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public T? Data { get; set; }
        public int StatusCode { get; set; }

        public GeneralResponse() { }
        public GeneralResponse(bool success, string? message, T? data, int statusCode)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }

    }
}
