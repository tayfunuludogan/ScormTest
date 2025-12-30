using Scorm.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Entities
{
    public class ContentAttempt
    {

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public ContentStandard Standard { get; set; }   // Attempt seviyesinde de tutmak pratik
        public int AttemptNo { get; set; }
        public AttemptStatus Status { get; set; } = AttemptStatus.InProgress;
        public decimal? Score { get; set; }             // Normalize: örn 0..100
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedAt { get; set; }
        public DateTime? LastActivityAt { get; set; }   // SCORM commit veya xAPI statement zamanı

        // Navigation
        public string? LaunchToken { get; set; }        // opsiyonel: launch güvenliği / korelasyon

        public User User { get; set; } = null!;

        // 1-1 Summaries
        public ContentAttemptScormSummary? ScormSummary { get; set; }
        public ContentAttemptXapiSummary? XapiSummary { get; set; }
        
    }
}
