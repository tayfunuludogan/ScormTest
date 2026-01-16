using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using Scorm.Business.Services.Abstract;

namespace WebAPI.Controllers
{
    [Route("api/scorm12")]
    [ApiController]
    public class Scorm12Controller : ControllerBase
    {
        IScormRuntimeService _runtime;
        public Scorm12Controller(IScormRuntimeService runtime)
        {
            _runtime = runtime;
        }

        [HttpPost("commit/{attemptId:guid}")]
        public async Task<IActionResult> Commit(Guid attemptId, [FromBody] Dictionary<string, string> data)
        {
            var result = await _runtime.HandleCommitAsync(attemptId, data);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("state/{attemptId:guid}")]
        public async Task<ActionResult<Dictionary<string, string>>> GetState(Guid attemptId)
        {
            var result = await _runtime.GetStateAsync(attemptId);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

    }
}
