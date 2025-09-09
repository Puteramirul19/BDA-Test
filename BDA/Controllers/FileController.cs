using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BDA.FileStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BDA.Web.Controllers
{
    //[Authorize]
    public class FileController : Controller
    {
        IFileStore fileStore;

        public FileController(IFileStore fileStore)
        {
            this.fileStore = fileStore;
        }

        public IActionResult Index(string id)
        {
            if (String.IsNullOrEmpty(id))
                return Forbid(); // Status 403

            try
            {
                var stream = fileStore.GetStream(id);
                var fileName = System.IO.Path.GetFileName(id);

                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                string contentType;
                if (!provider.TryGetContentType(fileName, out contentType))
                {
                    contentType = "application/octet-stream";
                }

                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);  // Status 404
            }
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            //// Simulate long uploads
            //System.Threading.Thread.Sleep(3000);

            var fileName = System.IO.Path.GetFileName(file.FileName);
            var fileId = DateTime.Now.Ticks.ToString() + "_" + fileName;

            object result = null;
            try
            {
                fileStore.Save(fileId, file.OpenReadStream());
                result = new
                {
                    fileName = fileName,
                    fileId = fileId,
                    fileUrl = Url.RouteUrl("Default", new { action = "Index", controller = "File", id = fileId }),
                    fileExtension = System.IO.Path.GetExtension(fileName),
                    fileSize = file.Length
                };
            }
            catch (System.IO.IOException)
            {
                // If file already exist, just ignore this exception.
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                result = new
                {
                    error = ex.Message
                };
            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult UploadBase64(string name, string extension, string data)
        {
            var fileId = DateTime.Now.Ticks.ToString() + "_" + name + "." + extension;

            object result = null;
            try
            {
                byte[] bytes = Convert.FromBase64String(data);
                var ms = new MemoryStream(bytes);
                fileStore.Save(fileId, ms);
                result = new
                {
                    fileName = name,
                    fileId = fileId,
                    fileUrl = Url.RouteUrl("Default", new { action = "Index", controller = "File", id = fileId }),
                    fileExtension = System.IO.Path.GetExtension(extension),
                    //fileSize = file.Length
                };
                ms.Close();
            }
            catch (System.IO.IOException)
            {
                // If file already exist, just ignore this exception.
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                result = new
                {
                    error = ex.Message
                };
            }

            return Json(result);
        }


        [HttpPost]
        public IActionResult Remove(string[] fileId)
        {
            foreach (var id in fileId)
            {
                fileStore.Delete(id);
            }

            return Json(new { });
        }
    }
}