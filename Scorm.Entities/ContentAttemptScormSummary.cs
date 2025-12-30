using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Entities
{
    public class ContentAttemptScormSummary
    {
        public Guid AttemptId { get; set; }
        

        // SCORM 1.2: cmi.core.lesson_status
        // SCORM 2004: cmi.completion_status + cmi.success_status
        public string? RawLessonStatus { get; set; }        // 1.2 için
        public string? RawCompletionStatus { get; set; }    // 2004 için
        public string? RawSuccessStatus { get; set; }       // 2004 için
        public string? RawExitMode { get; set; }            // 1.2: cmi.core.exit


        // Bookmark
        public string? LastLocation { get; set; }           // 1.2: cmi.core.lesson_location | 2004: cmi.location
        public string? SuspendData { get; set; }            // 1.2: cmi.suspend_data | 2004: cmi.suspend_data

       
        // Score hamları (istersen)
        public decimal? ScoreRaw { get; set; }              // 1.2: cmi.core.score.raw | 2004: cmi.score.raw
        public decimal? ScoreScaled { get; set; }           // 2004: cmi.score.scaled (0..1)
        public decimal? ScoreMin { get; set; }
        public decimal? ScoreMax { get; set; }

        
        // Time hamları (format string olabilir)
        public string? SessionTime { get; set; }            // 1.2: cmi.core.session_time | 2004: cmi.session_time
        public string? TotalTime { get; set; }              // 1.2: cmi.core.total_time | 2004: cmi.total_time

        public DateTime? LastCommitAt { get; set; }         // commit geldiği an



        public ContentAttempt Attempt { get; set; } = null!;

    }
}
