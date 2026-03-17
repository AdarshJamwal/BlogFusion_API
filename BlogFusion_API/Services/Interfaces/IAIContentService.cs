namespace BlogFusion_API.Services.Interfaces
{
    public interface IAIContentService
    {
        // Calls Google Gemini API and returns generated blog content as a string.
        // This mirrors the main(prompt) function in your Node.js gemini.js file.
        Task<string> GenerateContentAsync(string prompt);
    }
}