using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoAPI.Services;
using RabbitMQ.Client.Exceptions;

namespace VideoAPI.Controllers
{
    [Route("api/video")]
    [ApiController]
    public class VideoController : ControllerBase
    {

        private readonly FileService fileService;

        public VideoController(FileService fileService)
        {
            this.fileService = fileService;
        }



        // upload file(s) to server that palce under path: rootDirectory/subDirectory
        [HttpPost()]
        public IActionResult UploadFile([FromForm(Name = "files")] List<IFormFile> files, [FromForm(Name = "albumTitle")] string albumTitle, [FromForm(Name = "albumOwner")] string owner)
        {
            try
            {
                string blobName = $"{owner}-{albumTitle}";
                var location = fileService.SaveFile(files, albumTitle);
                fileService.UploadVideoToBlobStorage(location, blobName, owner, albumTitle).Wait();
                fileService.AddJobToMQ($"{blobName} {albumTitle} {owner}");
                fileService.DeleteFileAndDirectory(location);


                return Ok(new { files.Count, Size = FileService.SizeConverter(files.Sum(f => f.Length)) });
            }
            catch (BrokerUnreachableException e)
            {
                return BadRequest($"No service available, try again later");
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message} {albumTitle}:{owner}  {files}");
            }
        }
    }
}

