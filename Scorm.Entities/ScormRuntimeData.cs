using Scorm.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Entities
{
    public class ScormRuntimeData : IEntity
    {
        public long Id { get; set; }
        public Guid AttemptId { get; set; }
        public string Element { get; set; } = null!; // "cmi.core.lesson_status"
        public string? Value { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
