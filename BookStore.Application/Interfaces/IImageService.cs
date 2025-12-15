using Microsoft.AspNetCore.Http;

namespace BookStore.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folderName);
    }

}
