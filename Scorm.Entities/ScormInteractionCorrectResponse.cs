using Scorm.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Entities
{
    public class ScormInteractionCorrectResponse : IEntity
    {
        public long Id { get; set; }
        public long InteractionId { get; set; }
        public int PatternIndex { get; set; }
        public string Pattern { get; set; } = null!;
    }
}
