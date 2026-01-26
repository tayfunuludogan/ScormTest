using Scorm.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Entities
{
    public class XapiActivityState : IEntity
    {
        public long Id { get; set; }
        public Guid AttemptId { get; set; }
        public Guid? Registration { get; set; }           // context.registration
        public string ActivityId { get; set; } = null!;   // query: activityId
        public string StateId { get; set; } = null!;      // query: stateId
        public string AgentHash { get; set; } = null!;    // sha256(normalized agent json)
        public string? AgentJson { get; set; }            // optional audit/debug
        public string? ContentType { get; set; }          // e.g. application/json
        public byte[] Data { get; set; } = null!;         // binary/json/text
        public string? ETag { get; set; }                 // If-Match / ETag concurrency
        public DateTime UpdatedAt { get; set; }


        public ContentAttempt Attempt { get; set; } = null!;
    }
}
