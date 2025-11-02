using CustomerSuppTicket.Common.Intefaces.Services;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CustomerSuppTicket.Services.Services.AiServices
{
    public class MistralSummarizerClient : ISummarizerService
    {
        MistralOptions options;
        private readonly HttpClient _httpClient;

        public MistralSummarizerClient(IOptions<MistralOptions> options)
        {

            this.options = options.Value;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(this.options.baseUrl)
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.options.apiKey);
        }

        public async Task<string> SummarizeFaultDescriptionAsync(string faultDescription)
        {
            var requestBody = new
            {
                model = options.modelName,
                messages = new[]
                {
                    new { role = "user", content = $"Please summarize the following maintenance issue in a concise manner:\n{faultDescription}" }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(options.baseUrl, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var summary = doc.RootElement
                             .GetProperty("choices")[0]
                             .GetProperty("message")
                             .GetProperty("content")
                             .GetString();

            return summary;
        }
    }
}
