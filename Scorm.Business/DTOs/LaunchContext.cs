using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.DTOs
{
    public class LaunchContext
    {
        public string LaunchUrl { get; set; } = null!;  // iframe src
        public Dictionary<string, string> QueryParameters { get; set; } = new();
        public Dictionary<string, string> Headers { get; set; } = new(); 

        // JS tarafına geçecek config (window.RUNTIME_CONFIG vb. için)
        public Dictionary<string, object> ClientConfig { get; set; } = new();
    }
}
