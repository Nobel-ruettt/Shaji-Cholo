using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace CholoShajiCore.CoreServices
{
    public interface ICloudinaryService
    {
        ImageUploadResult UploadFile(IFormFile file);
        Cloudinary Create();
        bool DeleteFile(string publicId);
    }
}