using Scorm.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Entities
{
    public class ContentAttemptXapiSummary : IEntity
    {
        public Guid AttemptId { get; set; }
        // Actor özet (opsiyonel ama faydalı)
        public string? ActorMbox { get; set; } // mailto:...
        public string? ActorAccountHomePage { get; set; } //kimliği veren sistem
        public string? ActorAccountName { get; set; } //o sistemdeki benzersiz kullanıcı id

        // Completion / success kaynağı
        public string? CompletionVerbId { get; set; }       // completed/passed/failed vb.
        public DateTime? CompletionAt { get; set; }

        // Skor hamları (opsiyonel)
        public decimal? ScoreScaled { get; set; } // 0..1
        public decimal? ScoreRaw { get; set; }
        public decimal? ScoreMin { get; set; }
        public decimal? ScoreMax { get; set; }

        public DateTime? LastStatementAt { get; set; }      // son statement zamanı

        // İstersen “resume state” için
        public string? ResumeStateId { get; set; }          // örn "resume"


        public ContentAttempt Attempt { get; set; } = null!;


        #region Comments
        // xAPI attempt korelasyonu için en önemli alan
        //public Guid? Registration { get; set; }             // context.registration 
        #endregion

    }

}
