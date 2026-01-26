using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Models
{
    public record ParsedXapiStatement(
        Guid StatementId,
        Guid? Registration,     // == AttemptId bekliyoruz
        string VerbId,
        string ObjectId,
        string ActorJson,
        string? ResultJson,
        string? ContextJson,
        string RawJson,
        DateTime TimestampUtc,
        string? ActorMbox,
        string? ActorAccountHomePage,
        string? ActorAccountName,
        decimal? ScoreScaled,
        decimal? ScoreRaw,
        decimal? ScoreMin,
        decimal? ScoreMax
    );
}
