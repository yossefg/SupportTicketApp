using CustomerSuppTicket.Services.Options;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CustomerSuppTicket.Services.Services
{
    // SendGrid-based implementation of IEmailSender that reads credentials from configuration/environment
    public class MailKitEmailSender : IEmailSender
    {
        private readonly SendGridOptions options;
        private readonly string _templateRoot;

        public MailKitEmailSender(IOptions<SendGridOptions> options)
        {
            this.options = options.Value;
            _templateRoot = Path.Combine(AppContext.BaseDirectory, "EmailTemplets");

        }
        private string LoadTemplate(string filename)
        {
            var path = Path.Combine(_templateRoot, filename);
            return File.ReadAllText(path);
        }

        private string FillTemplate(string template, IDictionary<string, string> values)
        {
            foreach (var kv in values)
            {
                template = template.Replace("{{" + kv.Key + "}}", kv.Value);
            }
            return template;
        }

        public async Task SendEmailAsync(string name ,string ticketId, string toEmail)
        {

            var template = LoadTemplate("NewTicketMail.html");

            var filled = FillTemplate(template, new Dictionary<string, string>
            {
                ["CustomerName"] = name,
                ["TicketNumber"] = ticketId,
                ["SystemLink"] = options.LinkToSystemPrefix + ticketId
            });


            var client = new SendGridClient(options.ApiKey);

            var from = new EmailAddress(options.FromEmail, "Support Ticket");
            var to = new EmailAddress(toEmail);


            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                "Your Support Ticket Was Received",
                plainTextContent: null,
                htmlContent: filled
            );

            var response = await client.SendEmailAsync(msg);

            // Optionally, you can inspect response.StatusCode and log failures
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted && response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                // swallow or throw depending on your needs; here we choose to throw to surface the problem
                var respBody = await response.Body.ReadAsStringAsync();
                throw new InvalidOperationException($"SendGrid send failed: {response.StatusCode} - {respBody}");
            }
        }
    }

}


