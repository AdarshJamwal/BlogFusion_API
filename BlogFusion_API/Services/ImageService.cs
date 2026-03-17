using BlogFusion_API.Services.Interfaces;
using Imagekit.Sdk;
using Imagekit.Models;

namespace BlogFusion_API.Services
{
    public class ImageService : IImageService
    {
        private readonly ImagekitClient _imagekit;

        public ImageService(IConfiguration configuration)
        {
            var pub = configuration["ImageKit:PublicKey"] ?? throw new Exception("ImageKit:PublicKey missing");
            var priv = configuration["ImageKit:PrivateKey"] ?? throw new Exception("ImageKit:PrivateKey missing");
            var url = configuration["ImageKit:UrlEndpoint"] ?? throw new Exception("ImageKit:UrlEndpoint missing");

            _imagekit = new ImagekitClient(pub, priv, url);
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            using var ms = new MemoryStream();
            await imageFile.CopyToAsync(ms);

            var uploadReq = new FileCreateRequest
            {
                file = Convert.ToBase64String(ms.ToArray()),
                fileName = imageFile.FileName,
                folder = "/blogs"
            };

            var result = await _imagekit.UploadAsync(uploadReq);

            if (result == null || string.IsNullOrEmpty(result.url))
                throw new Exception("ImageKit upload failed.");

            // Append transformation params directly to URL — works with all SDK versions
            return $"{result.url}?tr=w-1280,q-80,f-webp";
        }
    }
}