using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Entities
{
    public class ScormInteractionObjective
    {
        public long Id { get; set; }
        public long InteractionId { get; set; }
        public int ObjectiveIndex { get; set; }
        public string ObjectiveId { get; set; } = null!;
    }
}
