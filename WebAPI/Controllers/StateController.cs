using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Scorm.Business.Models;
using Scorm.Business.Security;
using Scorm.Business.Services.Abstract;
using System.Buffers.Text;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.X86;
using WebAPI.Services;


namespace WebAPI.Controllers
{


    [ApiController]
    [Route("xapi/activities/state")]
    public class StateController : ControllerBase
    {
        //private readonly StateStore _store;
        private readonly IXapiRuntimeService _xapiRuntimeService;
        public StateController(
            IXapiRuntimeService xapiRuntimeService
            )
        {
            _xapiRuntimeService = xapiRuntimeService;
        }

        [HttpPost,HttpPut]
        public async Task<IActionResult> Put(
        [FromQuery] string activityId,
        [FromQuery] string agent,
        [FromQuery] string stateId,
        [FromQuery] Guid registration,
        CancellationToken ct)
        {
            
            using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms, ct);

            var contentType = string.IsNullOrWhiteSpace(Request.ContentType)
                ? "application/octet-stream"
                : Request.ContentType;

            var ifMatch = Request.Headers.IfMatch.ToString();
            if (string.IsNullOrWhiteSpace(ifMatch)) ifMatch = null;

            await _xapiRuntimeService.PutStateAsync(new XapiStateRequest(activityId, agent, stateId, registration),
                ms.ToArray(), contentType, ifMatch, ct);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> Get(
        [FromQuery] string activityId,
        [FromQuery] string agent,
        [FromQuery] string stateId,
        [FromQuery] Guid registration,
        CancellationToken ct)
        {
            var result = await _xapiRuntimeService.GetStateAsync(new XapiStateRequest(activityId, agent, stateId, registration), ct);
            if (result == null || !result.Success) return NotFound();

            var data = result.Data!;

            Response.Headers.ETag = data.ETag;
            Response.Headers.LastModified = data.UpdatedAtUtc.ToString("R");

            return File(data.Data, data.ContentType);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(
        [FromQuery] string activityId,
        [FromQuery] string agent,
        [FromQuery] string stateId,
        [FromQuery] Guid registration,
        CancellationToken ct)
        {
            var ifMatch = Request.Headers.IfMatch.ToString();
            if (string.IsNullOrWhiteSpace(ifMatch)) ifMatch = null;

            await _xapiRuntimeService.DeleteStateAsync(new XapiStateRequest(activityId, agent, stateId, registration), ifMatch, ct);
            return NoContent();
        }



        #region FirstStateController
        //private string GetKey()
        //{
        //    return $"{Request.Query["activityId"]}|" +
        //           $"{Request.Query["agent"]}|" +
        //           $"{Request.Query["registration"]}|" +
        //           $"{Request.Query["stateId"]}";
        //}

        ////✅ PUT
        ////Ne zaman?
        ////Kurs içinde ilerledikçe(slide geçişi, checkpoint, exit)
        ////Tarayıcı kapanmadan hemen önce(pakete göre)
        ////“Suspend/resume data” güncellendiğinde
        ////Body ne olur?
        ////Çoğu zaman JSON değil; bazen text/bytes(base64/deflate gibi) olabilir.
        ////O yüzden controller’da raw bytes saklamak doğru yaklaşım.

        ////✅ POST
        ////Bazı runtime’lar PUT yerine POST kullanabiliyor.
        ////Ne zaman?+
        ////“Merge/update” gibi davranış amaçlı(uygulamaya bağlı)
        ////Storyline’da bazen görünebilir
        ////Pratikte: PUT/POST ikisini de aynı şekilde kaydet.

        //[HttpPut, HttpPost]
        //public async Task<IActionResult> Save()
        //{
        //    if (!BasicAuthHandler.IsAuthorized(Request))
        //        return Unauthorized();

        //    var key = GetKey();

        //    using var ms = new MemoryStream();
        //    await Request.Body.CopyToAsync(ms);

        //    _store.Save(key, Request.ContentType ?? "application/octet-stream", ms.ToArray());
        //    return NoContent();
        //}





        ////Ne zaman?
        ////Kurs ilk açıldığında: “kullanıcının daha önce kaldığı yer var mı?”
        ////“Resume prompt var mı?” gibi kararlar için
        ////Sonuç bekleneni:
        ////200 + body → resume data var, kaldığın yerden devam edebilir
        ////404 → yok, sıfırdan başla
        ////Storyline bu yüzden açılışta genelde ilk iş GET stateId=resume yapar.

        //[HttpGet]
        //public IActionResult Get()
        //{
        //    if (!BasicAuthHandler.IsAuthorized(Request))
        //        return Unauthorized();

        //    var key = GetKey();
        //    var state = _store.Get(key);
        //    if (state == null)
        //        return NotFound();

        //    return File(state.Value.Data, state.Value.ContentType);
        //}


        ////Ne zaman?
        ////“Reset progress” gibi bir aksiyon varsa
        ////Yeni attempt başlatırken eski resume’u temizlemek isterse
        ////(Storyline’da her zaman gelmeyebilir ama desteklemek iyi)

        //[HttpDelete]
        //public IActionResult Delete()
        //{
        //    if (!BasicAuthHandler.IsAuthorized(Request))
        //        return Unauthorized();

        //    _store.Remove(GetKey());
        //    return NoContent();
        //} 
        #endregion
    }

}
