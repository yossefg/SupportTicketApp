namespace CustomerSuppTicket.Services.Services
{
 public interface IEmailSender
 {
 Task SendEmailAsync(string name, string ticketId, string toEmail);
 }
}
