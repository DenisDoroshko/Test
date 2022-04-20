using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagement.EventManagerApi.Services.Interfaces
{
    /// <summary>
    /// Provides methods for working with base64 images.
    /// </summary>
    public interface IImageProcesser
    {
        /// <summary>
        /// Gets png image in base64 format.
        /// </summary>
        /// <param name="imagePath">Image path.</param>
        /// <returns>Image in base64 format.</returns>
        Task<string> GetBase64Image(string imagePath);

        /// <summary>
        /// Saves base64 image as png.
        /// </summary>
        /// <param name="imagePath">Image path.</param>
        /// <param name="image">Image in base64 format.</param>
        /// <returns>Result of operation.</returns>
        Task<bool> SaveBase64Image(string imagePath, string image);
    }
}
