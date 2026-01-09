using Scorm.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Entities
{
    public class ScormInteraction : IEntity
    {
        public long Id { get; set; }
        public Guid AttemptId { get; set; }
        public int InteractionIndex { get; set; } // 0,1,2...
        public string? InteractionId { get; set; } // cmi.interactions.n.id
        public string? Type { get; set; } // choice, fill-in...
        public string? Result { get; set; } // correct, wrong...
        public decimal? Weighting { get; set; } //Bu etkileşimin puan ağırlığını belirtir.
        public string? Latency { get; set; } //Öğrencinin soruya cevap vermesi için geçen süreyi belirtir.
        public string? Time { get; set; } //Etkileşimin gerçekleştiği zaman (timestamp).
        public string? StudentResponse { get; set; } //Öğrencinin verdiği cevap.
    }
}
