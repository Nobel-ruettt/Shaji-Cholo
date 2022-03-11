using System.Composition;
using CholoShajiCore.Config;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace CholoShajiCore.CoreServices
{
    [Export(typeof(ICloudinaryService))]
    public class CloudinaryService : ICloudinaryService
    {
        private Cloudinary _cloudinary;
        private readonly object LockObject = new object();
        public  Cloudinary Cloudinary
        {
            get
            {
                if (_cloudinary == null)
                {
                    lock (LockObject)
                    {
                        if (_cloudinary == null)
                        {
                            _cloudinary = Create();
                        }
                    }
                }

                return _cloudinary;
            }
            set => _cloudinary = value;
        }

        public Cloudinary Create()
        {
            var acc = new Account(
                ConfigurationHelper.Instance.CloudinarySetting.CloudName,
                ConfigurationHelper.Instance.CloudinarySetting.ApiKey,
                ConfigurationHelper.Instance.CloudinarySetting.ApiSecret
            );
            return new Cloudinary(acc);
        }
        public ImageUploadResult UploadFile(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, stream)
                };
                uploadResult = Cloudinary.Upload(uploadParams);
            }
            return uploadResult;
        }

        public bool DeleteFile(string publicId)
        {
            var destroyParam = new DeletionParams(publicId);
            Cloudinary.Destroy(destroyParam);
            return true;
        }
    }
}
