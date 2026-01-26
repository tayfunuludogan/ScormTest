using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Helpers
{
    public class XapiETagHelper
    {
        public static string NewETag() => $"\"{Guid.NewGuid():N}\"";

        // If-Match header birden fazla etag içerebilir: "etag1", "etag2" veya *
        public static bool IfMatchSatisfied(string? ifMatchHeader, string currentEtag)
        {
            if (string.IsNullOrWhiteSpace(ifMatchHeader))
                return true; // If-Match yoksa izin ver (upsert)

            if (ifMatchHeader.Trim() == "*")
                return true;

            // Basit parse: virgülle ayrılabilir
            var parts = ifMatchHeader.Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            return parts.Any(p => string.Equals(p, currentEtag, StringComparison.Ordinal));
        }
    }
}
