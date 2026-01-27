using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Scorm.Business.Security;
using Scorm.Business.Services.Abstract;
using System;
using System.Text.Json;
using WebAPI.Services;

namespace WebAPI.Controllers
{


    [ApiController]
    [Route("xapi/statements")]
    public class StatementsController : ControllerBase
    {
        //private readonly StatementStore _store;
        private readonly IXapiRuntimeService _xapiRuntimeService;
        public StatementsController(
            IXapiRuntimeService xapiRuntimeService
            )
        {
            _xapiRuntimeService = xapiRuntimeService;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JsonElement payload, CancellationToken ct)
        {
            var ids = await _xapiRuntimeService.StoreAsync(payload, ct);
            return Ok(ids);
        }

        // PUT: client statementId belirler (idempotent)
        [HttpPut]
        public async Task<IActionResult> Put([FromQuery] Guid statementId, [FromBody] JsonElement statement, CancellationToken ct)
        {
            await _xapiRuntimeService.StoreWithIdAsync(statementId, statement, ct);
            return NoContent();
        }




        #region FirstStatementController
        ////Storyline’ın en sık yaptığıdır.
        ////Ne zaman?
        ////Kurs açılınca ilk event’ler (attempted/initialized benzeri)
        ////Slide/scene görüntülenince(experienced)
        ////Quiz sorusu cevaplanınca(answered)
        ////Kurs tamamlanınca(completed)
        ////Sınav sonucu oluşunca(passed/failed, result.score vs)
        ////Zaman/progress gibi ara kayıtlar(pakete göre)
        ////Neden PUT?
        ////Storyline statement’a kendi id’sini verip “ben bu statement’ı şu ID ile yazıyorum” der.
        ////xAPI’de bu akış normaldir.
        ////Request body: statement JSON (senin paylaştığın gibi)

        //[HttpPut]
        //public async Task<IActionResult> Put([FromQuery] string statementId)
        //{
        //    if (!BasicAuthHandler.IsAuthorized(Request))
        //        return Unauthorized();

        //    if (string.IsNullOrWhiteSpace(statementId))
        //        return BadRequest("statementId is required");

        //    using var reader = new StreamReader(Request.Body);
        //    var body = await reader.ReadToEndAsync();

        //    _store.Save(statementId, body);
        //    return NoContent();
        //}



        ////Bazı paketler/versiyonlar bunu da kullanabilir.
        ////Ne zaman?
        ////Runtime statementId üretmek istemezse
        ////Birden fazla statement’ı array olarak yollarsa
        ////Response (beklenen):
        ////["id1", "id2", ...]
        ////Storyline çoğunlukla PUT görürsün ama POST da mümkün.

        //[HttpPost]
        //public async Task<IActionResult> Post()
        //{
        //    if (!BasicAuthHandler.IsAuthorized(Request))
        //        return Unauthorized();

        //    using var reader = new StreamReader(Request.Body);
        //    var body = await reader.ReadToEndAsync();

        //    var id = Guid.NewGuid().ToString();
        //    _store.Save(id, body);

        //    // xAPI spec: POST -> statementIds array
        //    return Ok(new[] { id });
        //}


        ////Storyline genelde çok az kullanır; daha çok LRS raporlama içindir.
        ////Ne zaman?
        ////Nadiren bir statement’ı doğrulamak/okumak isterse(pakete göre)
        ////Bazı wrapper/launcher senaryolarında “kayıt var mı?” kontrolü

        //[HttpGet]
        //public IActionResult Get()
        //{
        //    if (!BasicAuthHandler.IsAuthorized(Request))
        //        return Unauthorized();

        //    return Ok(_store.All());
        //} 
        #endregion
    }

}
