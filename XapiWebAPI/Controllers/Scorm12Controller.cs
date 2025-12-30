using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;

namespace WebAPI.Controllers
{
    [Route("api/scorm12")]
    [ApiController]
    public class Scorm12Controller : ControllerBase
    {
        public Scorm12Controller()
        {
            
        }

        [HttpPost("commit/{attemptId:guid}")]
        public async Task<IActionResult> Commit(Guid attemptId, [FromBody] Dictionary<string, string> data)
        {
            //await _runtime.HandleCommitAsync(attemptId, data);
            return Ok();
        }

    }
}
