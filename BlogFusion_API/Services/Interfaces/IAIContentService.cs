namespace BlogFusion_API.Services.Interfaces
{
    public interface IAIContentService
    {
      
        Task<string> GenerateContentAsync(string prompt);
    }
}
