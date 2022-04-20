using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using TicketManagement.EventManagerApi.Services.Interfaces;

namespace TicketManagement.EventManagerApi.Services
{
    /// <summary>
    /// Provides methods for working with base64 images.
    /// </summary>
    public class ImageProcesser : IImageProcesser
    {
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProcesser"/> class.
        /// </summary>
        /// <param name="environment">Environment.</param>
        public ImageProcesser(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        /// <inheritdoc />
        public async Task<string> GetBase64Image(string imagePath)
        {
            try
            {
                var path = Path.Combine(_environment.WebRootPath, imagePath);
                using (var memoryStream = new MemoryStream())
                {
                    using (var stream = System.IO.File.OpenRead(path))
                    {
                        await stream.CopyToAsync(memoryStream);
                        return "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<bool> SaveBase64Image(string imagePath, string image)
        {
            var isSuccess = true;
            try
            {
                using (Stream fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    var bytes = Convert.FromBase64String(image);
                    await fileStream.WriteAsync(bytes);
                }
            }
            catch
            {
                isSuccess = false;
            }

            return isSuccess;
        }
    }
}
