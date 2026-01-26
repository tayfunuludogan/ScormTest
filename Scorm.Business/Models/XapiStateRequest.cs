using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Models
{
    public record XapiStateRequest(
        string ActivityId,
        string AgentJson,
        string StateId,
        Guid Registration
    );
}
