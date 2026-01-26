using Scorm.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Entities
{
    public class XapiStatement : IEntity
    {
        public long Id { get; set; }
        public Guid AttemptId { get; set; }         // eşleştirebilirsen doldurursun

        public string VerbId { get; set; } = null!;
        public string ObjectId { get; set; } = null!;
        public string ActorJson { get; set; } = null!;

        public string? ResultJson { get; set; }
        public string? ContextJson { get; set; }
        public string RawJson { get; set; } = null!;

        public DateTime Timestamp { get; set; } //Olayın gerçekleştiği an (offline da)
        public DateTime StoredAt { get; set; } //Statement’ın LRS’e kaydedildiği zaman
    }

}
