using ClosedXML.Excel;
using Microsoft.AspNetCore.Cors;
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

        [EnableCors("MyAllowSpecificOrigins")]
        [HttpPost]
        public async Task<IActionResult> PostFile(
            [FromForm] IFormFile file)
        {
            // To be sure about CORS support
            Response.Headers.AccessControlAllowOrigin = "https://localhost:7062";
            Response.Headers.AccessControlAllowHeaders = "multipart/form-data";
            Response.Headers.AccessControlAllowMethods = "POST";

            long maxFileSize = 1024 * 1000;
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");

            var uploadResult = new UploadResult();
            string trustedFileName, path;
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
                do
                {
                    // eg. [path]/[randomfilename]-20180828_145811-456.xlsx
                    trustedFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + "-" +
                       DateTime.Now.ToString("yyyyMMdd_HHmmss-fff") + ".xlsx";

                    path = Path.Combine(
                        env.ContentRootPath,
                        env.EnvironmentName,
                        "Uploads",
                        trustedFileName
                    );
                } while (System.IO.File.Exists(path));

                logger.LogInformation("Selected path: {Path}", path);
                try
                {
                    await using FileStream fs = new(path, FileMode.Create);
                    await file.CopyToAsync(fs);

                    logger.LogInformation("{FileName} saved at {Path}",
                        trustedFileName,
                        resourcePath.ToString() + env.EnvironmentName + "/Upload"
                    );

                    uploadResult.Uploaded = true;
                    uploadResult.StoredFileName = trustedFileName;
                }
                catch (IOException ex)
                {
                    logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                        trustedFileName, ex.Message);
                    uploadResult.ErrorCode = 3;
                }
                catch (Exception ex)
                {
                    logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                        trustedFileName, ex.Message);
                    uploadResult.ErrorCode = 1;
                }

                try
                {
                    uploadResult.Data = GetParsedData(path);
                }
                catch (Exception ex)
                {
                    logger.LogError("parsing is shit: {exception}", ex.Message);
                    uploadResult.ErrorCode = 5;
                }
            }
            return Ok(uploadResult);
        }

        private Dictionary<string, List<string>> GetParsedData(string filePath)
        {
            var workbook = new XLWorkbook(filePath);
            if (workbook.IsProtected)
            {
                throw new Exception("The WorkBook is protected somehow. I won't open it sry");
            }
            if (workbook.IsPasswordProtected)
            {
                throw new Exception("The WorkBook is protected with a password");
            }
            if (workbook.Worksheets.Count == 0)
            {
                throw new Exception("WorkBook doesn't contain worksheets");
            }

            Dictionary<string, List<string>> output = new();
            var worksheet = workbook.Worksheet(1);

            var firstRowUsed = worksheet.FirstRowUsed();
            if (firstRowUsed is null)
            {
                throw new Exception("I guiess you have empty worksheet or smth");
            }

            foreach(var cell in firstRowUsed.Cells())
            {
                output[cell.GetValue<string>()] = new List<string>();
                //output["headers"].Add(cell.GetValue<string>());
            }



            //int lastrow = ws.LastRowUsed().RowNumber();
            //var rows = ws.Rows(1, lastrow);
            //foreach (IXLRow row in rows)
            //{
            //    if (row.IsEmpty())
            //        row.Delete();
            //}
            //foreach(var cell in categoryRow.Cells)
            //{

            //}

            //return new()
            //{
            //    ["This stuff FROM SERVER BROOOO"] = new List<string>() { "Tom", "Bob", "Sam", "buba", "pupa" },
            //    ["clients"] = new List<string>() { "Tom", "Bob", "Sam" },
            //    ["listeners"] = new List<string>() { "asdf", "sdfafsdas", "Sasdfafafsasm" }
            //};
            return output;
        }

    }
}
