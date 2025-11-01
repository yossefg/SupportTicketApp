namespace CustomerSuppTicket.Services.Options
{
    public class SendGridOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string LinkToSystemPrefix { get; set; } = string.Empty;
    }
}