using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Helpers
{
    public static class XapiVerbs
    {
        public static bool IsCompleted(string verbId)
            => verbId.EndsWith("/completed", StringComparison.OrdinalIgnoreCase);

        public static bool IsPassed(string verbId)
            => verbId.EndsWith("/passed", StringComparison.OrdinalIgnoreCase);

        public static bool IsFailed(string verbId)
            => verbId.EndsWith("/failed", StringComparison.OrdinalIgnoreCase);
    }

}
