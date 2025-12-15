using BookStore.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BookStore.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly string _baseImagePath;

        public ImageService(IConfiguration configuration)
        {
            _baseImagePath = Path.Combine(Directory.GetCurrentDirectory(), configuration["ImageSettings:ImageUploadPath"]);
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string folderName)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var directoryPath = Path.Combine(_baseImagePath, folderName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(directoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/images/{folderName}/{fileName}";
        }
    }
}
