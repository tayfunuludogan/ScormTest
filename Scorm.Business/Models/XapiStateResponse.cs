using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Models
{
    public class XapiStateResponse
    {
        public byte[] Data { get; set; } = null!;
        public string ContentType { get; set; } = "application/octet-stream";
        public string ETag { get; set; } = "\"\"";
        public DateTime UpdatedAtUtc { get; set; }
    }
}
