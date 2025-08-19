using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var cloudName = config["CloudinarySettings:CloudName"];
            var apiKey = config["CloudinarySettings:ApiKey"];
            var apiSecret = config["CloudinarySettings:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        // Upload image
        public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
        {
            try
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, imageStream)
                };

                var uploadResult = await Task.Run(() => _cloudinary.Upload(uploadParams));

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return uploadResult.Url.ToString(); // Return the image URL
                }
                else
                {
                    throw new Exception("Failed to upload image");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error uploading image: " + ex.Message);
            }
        }

        // Delete image from Cloudinary
        public async Task<bool> DeleteImageAsync(string publicId)
        {
            try
            {
                var deleteParams = new DeletionParams(publicId);
                var deletionResult = await Task.Run(() => _cloudinary.Destroy(deleteParams));

                return deletionResult.Result == "ok";
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting image: " + ex.Message);
            }
        }
    }
}
