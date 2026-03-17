using BlogFusion_API.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace BlogFusion_API.Services
{
    public class AIContentService : IAIContentService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        private const string Model = "gemini-2.0-flash";
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";

        // IHttpClientFactory is registered in Program.cs via builder.Services.AddHttpClient()
        public AIContentService(IHttpClientFactory factory, IConfiguration config)
        {
            _http = factory.CreateClient();
            _apiKey = config["Gemini:ApiKey"] ?? throw new Exception("Gemini:ApiKey missing");
        }

        public async Task<string> GenerateContentAsync(string prompt)
        {
            // Same suffix as Node.js: prompt + ' Generate a blog content for this topic...'
            var fullPrompt = $"{prompt} Generate a blog content for this topic in simple text format";

            // Build the request body in Gemini REST API format
            var body = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = fullPrompt } } }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(
                $"{BaseUrl}/{Model}:generateContent?key={_apiKey}", content);

            

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API error: {response.StatusCode} - {errorBody}");
            }

            // Parse response and navigate to: candidates[0].content.parts[0].text
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "No content generated.";
        }
    }
}