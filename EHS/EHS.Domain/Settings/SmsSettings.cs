namespace EHS.Domain.Settings
{
    public class SmsSettings
    {
        public bool Enabled { get; set; } = false;
        public string Provider { get; set; } = "LogOnly"; // Twilio, Azure, LogOnly
        public string AccountSid { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string FromNumber { get; set; } = string.Empty;

        // Azure Service Bus
        public bool UseAzureServiceBus { get; set; } = false;
        public string ServiceBusConnectionString { get; set; } = string.Empty;
        public string SmsQueueName { get; set; } = "sms-queue";

        // Azure Communication Services
        public string CommunicationServiceConnectionString { get; set; } = string.Empty;
    }
}
