using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Scorm.Business.Security;
using System.Buffers.Text;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.X86;
using WebAPI.Services;


namespace XapiWebAPI.Controllers
{


    [ApiController]
    [Route("xapi/activities/state")]
    public class StateController : ControllerBase
    {
        private readonly StateStore _store;

        public StateController(StateStore store)
        {
            _store = store;
        }

        private string GetKey()
        {
            return $"{Request.Query["activityId"]}|" +
                   $"{Request.Query["agent"]}|" +
                   $"{Request.Query["registration"]}|" +
                   $"{Request.Query["stateId"]}";
        }

        //✅ PUT
        //Ne zaman?
        //Kurs içinde ilerledikçe(slide geçişi, checkpoint, exit)
        //Tarayıcı kapanmadan hemen önce(pakete göre)
        //“Suspend/resume data” güncellendiğinde
        //Body ne olur?
        //Çoğu zaman JSON değil; bazen text/bytes(base64/deflate gibi) olabilir.
        //O yüzden controller’da raw bytes saklamak doğru yaklaşım.

        //✅ POST
        //Bazı runtime’lar PUT yerine POST kullanabiliyor.
        //Ne zaman?+
        //“Merge/update” gibi davranış amaçlı(uygulamaya bağlı)
        //Storyline’da bazen görünebilir
        //Pratikte: PUT/POST ikisini de aynı şekilde kaydet.

        [HttpPut, HttpPost]
        public async Task<IActionResult> Save()
        {
            if (!BasicAuthHandler.IsAuthorized(Request))
                return Unauthorized();

            var key = GetKey();

            using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);

            _store.Save(key, Request.ContentType ?? "application/octet-stream", ms.ToArray());
            return NoContent();
        }





        //Ne zaman?
        //Kurs ilk açıldığında: “kullanıcının daha önce kaldığı yer var mı?”
        //“Resume prompt var mı?” gibi kararlar için
        //Sonuç bekleneni:
        //200 + body → resume data var, kaldığın yerden devam edebilir
        //404 → yok, sıfırdan başla
        //Storyline bu yüzden açılışta genelde ilk iş GET stateId=resume yapar.

        [HttpGet]
        public IActionResult Get()
        {
            if (!BasicAuthHandler.IsAuthorized(Request))
                return Unauthorized();

            var key = GetKey();
            var state = _store.Get(key);
            if (state == null)
                return NotFound();

            return File(state.Value.Data, state.Value.ContentType);
        }


        //Ne zaman?
        //“Reset progress” gibi bir aksiyon varsa
        //Yeni attempt başlatırken eski resume’u temizlemek isterse
        //(Storyline’da her zaman gelmeyebilir ama desteklemek iyi)

        [HttpDelete]
        public IActionResult Delete()
        {
            if (!BasicAuthHandler.IsAuthorized(Request))
                return Unauthorized();

            _store.Remove(GetKey());
            return NoContent();
        }
    }

}
