namespace EHS.Domain.Settings
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; } = string.Empty;
        
        // Azure Service Bus
        public bool UseAzureServiceBus { get; set; } = false;
        public string ServiceBusConnectionString { get; set; } = string.Empty;
        public string EmailQueueName { get; set; } = "email-queue";

        // Azure Communication Services
        public string CommunicationServiceConnectionString { get; set; } = string.Empty;
    }
}
