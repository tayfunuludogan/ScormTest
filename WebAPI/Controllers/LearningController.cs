using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using Scorm.Business.Services.Abstract;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningController : ControllerBase
    {
        private readonly IScormLearningService _scormLearningService;
        public LearningController(
            IScormLearningService scormLearningService)
        {
            _scormLearningService = scormLearningService;
        }


        [HttpGet("packages")]
        public async Task<IActionResult> Packages()
        {
            var result = await _scormLearningService.GetPackages();
            if (result.Success)
                return Ok(result);

            return BadRequest(result.Message);
        }



        [HttpPost("launch/{packageId:guid}")]
        public async Task<IActionResult> Launch(Guid packageId)
        {
            var result = await _scormLearningService.BuildLaunchContext(packageId);
            if(result.Success)
                return Ok(result);

            return BadRequest(result.Message);
        }


        [HttpPost("scorm/commit/{attemptId:guid}")]
        public async Task<IActionResult> Commit(Guid attemptId, [FromBody] Dictionary<string, string> data)
        {
            var result = await _scormLearningService.HandleCommitAsync(attemptId, data);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("scorm/state/{attemptId:guid}")]
        public async Task<ActionResult<Dictionary<string, string>>> GetState(Guid attemptId)
        {
            var result = await _scormLearningService.GetStateAsync(attemptId);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }



    }
}
