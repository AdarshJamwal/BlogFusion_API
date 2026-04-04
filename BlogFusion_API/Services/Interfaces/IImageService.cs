namespace BlogFusion_API.Services.Interfaces
{
    public interface IImageService
    {
       
        Task<string> UploadImageAsync(IFormFile imageFile);
    }
}
