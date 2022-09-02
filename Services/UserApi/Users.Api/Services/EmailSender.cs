using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Users.Api.Services.SendGrid;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Users.Api.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        private readonly SendGridOptions _options;

        public EmailSender(
            ILogger<EmailSender> logger,
            IOptions<SendGridOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task SendEmailAsync(string emailAddress, string subject, string content)
        {
            var client = new SendGridClient(_options.ApiKey);
            var message = new SendGridMessage();
            message.SetFrom("ozmen_celik@hotmail.com", "Ozzy Ozmen Celik");
            message.SetSubject(subject);
            message.AddTo(new EmailAddress(emailAddress));
            message.HtmlContent = content;
            message.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(message);

            _logger.LogInformation(response.IsSuccessStatusCode
                                   ? $"Email to {emailAddress} queued successfully!"
                                   : $"Failure Email to {emailAddress}");
        }
    }
}