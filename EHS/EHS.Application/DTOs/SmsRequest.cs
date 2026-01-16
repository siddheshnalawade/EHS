namespace EHS.Application.DTOs
{
    public class SmsRequest
    {
        public string ToPhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
