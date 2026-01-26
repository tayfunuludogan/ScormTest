using Scorm.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Scorm.Business.Helpers
{
    public static class XapiStatementParser
    {
        public static ParsedXapiStatement Parse(JsonElement stmt, Guid statementId)
        {
            // verb.id
            var verbId = stmt.GetProperty("verb").GetProperty("id").GetString() ?? "";

            // object.id
            var obj = stmt.GetProperty("object");
            var objectId = obj.TryGetProperty("id", out var oid) ? (oid.GetString() ?? "") : "";

            // timestamp
            DateTime tsUtc = DateTime.UtcNow;
            if (stmt.TryGetProperty("timestamp", out var tsEl) && tsEl.ValueKind == JsonValueKind.String)
                tsUtc = DateTime.Parse(tsEl.GetString()!, null, System.Globalization.DateTimeStyles.AdjustToUniversal);

            // actor + actor identity
            var actorEl = stmt.GetProperty("actor");
            var actorJson = actorEl.GetRawText();

            string? mbox = null;
            string? accHome = null;
            string? accName = null;

            if (actorEl.TryGetProperty("mbox", out var mboxEl) && mboxEl.ValueKind == JsonValueKind.String)
                mbox = mboxEl.GetString();

            if (actorEl.TryGetProperty("account", out var accEl) && accEl.ValueKind == JsonValueKind.Object)
            {
                if (accEl.TryGetProperty("homePage", out var hp) && hp.ValueKind == JsonValueKind.String) accHome = hp.GetString();
                if (accEl.TryGetProperty("name", out var nm) && nm.ValueKind == JsonValueKind.String) accName = nm.GetString();
            }

            // context + registration
            string? contextJson = null;
            Guid? reg = null;
            if (stmt.TryGetProperty("context", out var ctxEl) && ctxEl.ValueKind == JsonValueKind.Object)
            {
                contextJson = ctxEl.GetRawText();
                if (ctxEl.TryGetProperty("registration", out var regEl) && regEl.ValueKind == JsonValueKind.String)
                    if (Guid.TryParse(regEl.GetString(), out var g)) reg = g;
            }

            // result + score
            string? resultJson = null;
            decimal? scaled = null; decimal? raw = null; decimal? min = null; decimal? max = null;

            if (stmt.TryGetProperty("result", out var resEl) && resEl.ValueKind == JsonValueKind.Object)
            {
                resultJson = resEl.GetRawText();
                if (resEl.TryGetProperty("score", out var scEl) && scEl.ValueKind == JsonValueKind.Object)
                {
                    if (scEl.TryGetProperty("scaled", out var s) && s.ValueKind == JsonValueKind.Number && s.TryGetDecimal(out var sd)) scaled = sd;
                    if (scEl.TryGetProperty("raw", out var r) && r.ValueKind == JsonValueKind.Number && r.TryGetDecimal(out var rd)) raw = rd;
                    if (scEl.TryGetProperty("min", out var mi) && mi.ValueKind == JsonValueKind.Number && mi.TryGetDecimal(out var mid)) min = mid;
                    if (scEl.TryGetProperty("max", out var ma) && ma.ValueKind == JsonValueKind.Number && ma.TryGetDecimal(out var mad)) max = mad;
                }
            }

            var rawJson = stmt.GetRawText();

            return new ParsedXapiStatement(
                statementId,
                reg,
                verbId,
                objectId,
                actorJson,
                resultJson,
                contextJson,
                rawJson,
                tsUtc,
                mbox,
                accHome,
                accName,
                scaled,
                raw,
                min,
                max
            );
        }
    }
}
