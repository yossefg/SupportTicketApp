namespace CustomerSuppTicket.Services.Services
{
    public class MistralOptions
    {
        public string apiKey { get; set; } = string.Empty;
        public string baseUrl { get; set; } = string.Empty;

        public string modelName { get; set; } = string.Empty;
    }
}