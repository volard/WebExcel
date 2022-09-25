﻿//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebExcel.Shared;

namespace WebExcel.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesaveController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ILogger<FilesaveController> logger;

        public FilesaveController(IWebHostEnvironment env,
            ILogger<FilesaveController> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<UploadResult>> PostFile(
            [FromForm] IFormFile file)
        {
            
            long maxFileSize = 1024 * 65;
            //var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");

            var uploadResult = new UploadResult();
            string trustedFileNameForFileStorage;
            var untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);
            logger.LogInformation(Request.ToString());

            if (file.Length == 0)
            {
                logger.LogInformation("{FileName} length is 0 (Err: 1)",
                    trustedFileNameForDisplay);
                uploadResult.ErrorCode = 1;
            }
            else if (file.Length > maxFileSize)
            {
                logger.LogInformation("{FileName} of {Length} bytes is " +
                    "larger than the limit of {Limit} bytes (Err: 2)",
                    trustedFileNameForDisplay, file.Length, maxFileSize);
                uploadResult.ErrorCode = 2;
            }
            else
            {
                try
                {
                    //trustedFileNameForFileStorage = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                    trustedFileNameForFileStorage = Path.GetRandomFileName();
                    var path = Path.Combine(env.ContentRootPath,
                        env.EnvironmentName, "Uploads",
                        trustedFileNameForFileStorage);
                    Console.WriteLine(path);

                    await using FileStream fs = new(path, FileMode.Create);
                    await file.CopyToAsync(fs);

                    logger.LogInformation("{FileName} saved at {Path}",
                        trustedFileNameForDisplay, path);

                    uploadResult.Uploaded = true;
                    uploadResult.StoredFileName = trustedFileNameForFileStorage;
                }
                catch (IOException ex)
                {
                    logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                        trustedFileNameForDisplay, ex.Message);
                    uploadResult.ErrorCode = 3;
                }
            }

            return uploadResult;
        }
    }
}
