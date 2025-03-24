using BaoTran.Models;
using BaoTran.Services;
using Microsoft.AspNetCore.Mvc;

namespace BaoTran.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaFileController : ControllerBase
    {
        private readonly IMediaFileService mediaFileService;

        public MediaFileController(IMediaFileService mediaFileService)
        {
            this.mediaFileService = mediaFileService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMediaFiles()
        {
            return Ok(await mediaFileService.GetAllMediaFiles());
        }


        [HttpGet("{idMediaFile}")]
        public async Task<IActionResult> GetSingleMediaFile(int idMediaFile)
        {
            MediaFileResponse mediaFile = await mediaFileService.GetSingleMediaFile(idMediaFile);
            if (mediaFile == null)
            {
                return BadRequest("MediaFile does not exist");
            }
            return Ok(mediaFile);
        }

        [HttpPost]
        public async Task<IActionResult> PostMediaFile([FromForm] MediaFileRequest mediaFile)
        {
            (bool Success, string ErrorMessage) result = await mediaFileService.PostMediaFile(mediaFile);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("Data added successfully");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutMediaFile(int id, [FromForm] MediaFileRequest mediaFileUpdate)
        {
            (bool Success, string ErrorMessage) result = await mediaFileService.PutMediaFile(id, mediaFileUpdate);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Data updated successfully");

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMediaFile(int id)
        {
            (bool Success, string ErrorMessage) result = await mediaFileService.DeleteMediaFile(id);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("Data deleted successfully");

        }

    }
}
