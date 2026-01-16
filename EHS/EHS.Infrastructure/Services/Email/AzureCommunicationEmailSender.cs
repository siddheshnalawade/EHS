using Azure;
using Azure.Communication.Email;
using EHS.Domain.Settings;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using Microsoft.Extensions.Options;

namespace EHS.Infrastructure.Services.Email
{
    public class AzureCommunicationEmailSender : ISender
    {
        private readonly EmailClient _emailClient;
        private readonly EmailSettings _settings;

        public AzureCommunicationEmailSender(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
            // Assumes ConnectionString is provided. 
            // In a real scenario, you might use TokenCredential (Managed Identity).
            _emailClient = new EmailClient(_settings.CommunicationServiceConnectionString);
        }

        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            // Sync version - wrapper around Async
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            try
            {
                var content = new EmailContent(email.Data.Subject)
                {
                    PlainText = email.Data.PlaintextAlternativeBody,
                    Html = email.Data.Body
                };

                // Azure Email requires a specific "DoNotReply@..." sender address provisioned in Azure
                var senderAddress = _settings.FromEmail; 

                var recipients = new EmailRecipients(
                    email.Data.ToAddresses.Select(a => new EmailAddress(a.EmailAddress, a.Name))
                );

                if (email.Data.CcAddresses.Any())
                {
                    foreach (var cc in email.Data.CcAddresses)
                    {
                        recipients.CC.Add(new EmailAddress(cc.EmailAddress, cc.Name));
                    }
                }

                if (email.Data.BccAddresses.Any())
                {
                    foreach (var bcc in email.Data.BccAddresses)
                    {
                        recipients.BCC.Add(new EmailAddress(bcc.EmailAddress, bcc.Name));
                    }
                }

                var message = new EmailMessage(senderAddress, recipients, content);

                // Send the email
                // WaitUntil.Started returns as soon as accepted. WaitUntil.Completed waits for delivery (can be slow).
                // For a background worker, Started is usually fine, but Completed gives definitive success.
                var operation = await _emailClient.SendAsync(WaitUntil.Completed, message, token ?? CancellationToken.None);

                return new SendResponse
                {
                    MessageId = operation.Id,
                    ErrorMessages = new List<string>() // No errors
                };
            }
            catch (RequestFailedException ex)
            {
                return new SendResponse
                {
                    ErrorMessages = new List<string> { $"Azure Email Error: {ex.ErrorCode} - {ex.Message}" }
                };
            }
            catch (Exception ex)
            {
                return new SendResponse
                {
                    ErrorMessages = new List<string> { ex.Message }
                };
            }
        }
    }
}
