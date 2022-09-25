//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
//using WebExcel.Shared;

namespace WebExcel.Server.Controllers
{
    public class UploadResult
    {
        public Dictionary<string, List<string>>? Data { get; set; }
        public bool Uploaded { get; set; }
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
        public int ErrorCode { get; set; }
    }


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

        [EnableCors("MyAllowSpecificOrigins")]
        [HttpPost]
        public async Task<IActionResult> PostFile(
            [FromForm] IFormFile file)
            //)
        {

            //logger.LogInformation(Request.QueryString.ToString());
            //logger.LogInformation(Request.Headers.ToString());
            //logger.LogInformation(Request.Method.ToString());

            //foreach (var header in Request.Headers)
            //{
            //    logger.LogInformation(header.Key + " : " + header.Value + "\n==================================");
            //}
            //logger.LogInformation("Uploaded actual filename" + " : " + file.FileName + "\n==================================");

            //logger.LogInformation("\n==================================");
            //logger.LogInformation("\n==================================");

            //foreach(var header in Response.Headers)
            //{
            //    logger.LogInformation(header.Key + " : " + header.Value + "\n==================================");
            //}

            //UploadResult res = new()
            //{
            //    FileName = "testName",
            //    Uploaded = true,
            //    StoredFileName = "StoredName.txt"
            //};


            







  //          env.response.headers["Access-Control-Allow-Methods"] = "POST"
  //# ...with `Content-type` header in the request...
  //env.response.headers["Access-Control-Allow-Headers"] = "Content-type"


            //return res;
            


            long maxFileSize = 1024 * 1000;
            //var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");

            var uploadResult = new UploadResult();
            //string trustedFileNameForFileStorage;
            var untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

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
                    //trustedFileNameForFileStorage = Path.GetRandomFileName();
                    //var path = Path.Combine(env.ContentRootPath,
                    //    env.EnvironmentName, "Uploads",
                    //    trustedFileNameForFileStorage);
                    //Console.WriteLine(path);

                    await using FileStream fs = new(env.ContentRootPath + "Development/Uploads/some.xlsx", FileMode.Create);
                    await file.CopyToAsync(fs);

                    //logger.LogInformation("{FileName} saved at {Path}",
                    //    trustedFileNameForDisplay, path);

                    //uploadResult.Uploaded = true;
                    //uploadResult.StoredFileName = trustedFileNameForFileStorage;
                }
                catch (IOException ex)
                {
                    logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                        "somename.txt", ex.Message);
                    uploadResult.ErrorCode = 3;
                }
                catch (Exception ex)
                {
                    logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                        "somename.txt", ex.Message);
                }
            }

            Response.Headers.AccessControlAllowOrigin = "https://localhost:7062"; // Allow-Origin
            Response.Headers.AccessControlAllowHeaders = "multipart/form-data";
            //Response.Headers.AccessControlAllowHeaders = ;
            Response.Headers.AccessControlAllowMethods = "POST";
            return Ok("asdf");
            //uploadResult.Data = new()
            //{
            //    ["IM FROM SERVER BROOOO"] = new List<string>() { "Tom", "Bob", "Sam", "biba", "pupa" },
            //    ["clients"] = new List<string>() { "Tom", "Bob", "Sam" },
            //    ["listeners"] = new List<string>() { "asdf", "sdfafsdas", "Sasdfafafsasm" }
            //};
            //return uploadResult;
        }
    }
}
