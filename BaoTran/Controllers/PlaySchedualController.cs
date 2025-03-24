using BaoTran.Models;
using BaoTran.Services;
using Microsoft.AspNetCore.Mvc;

namespace BaoTran.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaySchedualController : ControllerBase
    {
        private readonly IPlaySchedualService playSchedualService;

        public PlaySchedualController(IPlaySchedualService playSchedualService)
        {
            this.playSchedualService = playSchedualService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllPlaySchedual()
        {
            return Ok(await playSchedualService.GetAllPlaySchedual());
        }


        [HttpPost]
        public async Task<IActionResult> PostPlaySchedual([FromForm] PlaySchedualRequest playSchedualFile)
        {
            (bool Success, string ErrorMessage) result = await playSchedualService.PostPlaySchedual(playSchedualFile);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("Data added successfully");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaySchedual(int id, [FromForm] PlaySchedualRequest playSchedualUpdate)
        {
            (bool Success, string ErrorMessage) result = await playSchedualService.PutPlaySchedual(id, playSchedualUpdate);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("Data updated successfully");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaySchedual(int id)
        {
            (bool Success, string ErrorMessage) result = await playSchedualService.DeletePlaySchedual(id);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("Data deleted successfully");
        }
    }
}
