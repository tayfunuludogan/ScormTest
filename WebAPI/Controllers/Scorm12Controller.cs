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
        IScorm12RuntimeService _runtime;
        public Scorm12Controller(IScorm12RuntimeService runtime)
        {
            _runtime = runtime;
        }

        [HttpPost("commit/{attemptId:guid}")]
        public async Task<IActionResult> Commit(Guid attemptId, [FromBody] Dictionary<string, string> data)
        {
            await _runtime.HandleCommitAsync(attemptId, data);
            return Ok();
        }

        [HttpGet("state/{attemptId:guid}")]
        public async Task<ActionResult<Dictionary<string, string>>> GetState(Guid attemptId)
        {
            var state = await _runtime.GetStateAsync(attemptId);
            return Ok(state);
        }

    }
}
