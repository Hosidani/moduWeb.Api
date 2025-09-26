using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ModuWeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly string _blobConnectionString;
        private readonly string _blobContainerName;

        public ImageController()
        {
            // TODO: Move to config in production
            _blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=moduwebdevstorage;AccountKey=ipqW+1Cu05xbwEjYKkASLkvKxFrJvEvXLUGxY3hay7SIhgk8V5NV6wpJy0h3au0AhBA3Kl9FKryk+ASt43XWgw==;EndpointSuffix=core.windows.net";
            _blobContainerName = Environment.GetEnvironmentVariable("AZURE_BLOB_CONTAINER_NAME")
                ?? "product-image";
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Generate unique filename
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            try
            {
                var blobServiceClient = new BlobServiceClient(_blobConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(_blobContainerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobClient = containerClient.GetBlobClient(fileName);
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
                }

                var url = blobClient.Uri.ToString();
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Image upload failed: {ex.Message}");
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteImage([FromQuery] string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("No file name provided.");

            try
            {
                var blobServiceClient = new BlobServiceClient(_blobConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(_blobContainerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                var result = await blobClient.DeleteIfExistsAsync();
                if (result)
                    return Ok(new { deleted = true });
                else
                    return NotFound(new { deleted = false, message = "File not found in blob storage." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Image delete failed: {ex.Message}");
            }
        }
    }
}