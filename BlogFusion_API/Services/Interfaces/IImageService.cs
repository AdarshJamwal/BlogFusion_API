namespace BlogFusion_API.Services.Interfaces
{
    public interface IImageService
    {
        // Uploads the file to ImageKit CDN and returns the optimized public URL.
        // This mirrors the imagekit.upload() call in your Node.js imageKit.js file.
        Task<string> UploadImageAsync(IFormFile imageFile);
    }
}