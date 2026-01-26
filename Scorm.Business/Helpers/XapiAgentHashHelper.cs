using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Scorm.Business.Helpers
{
    public static class XapiAgentHashHelper
    {
        public static string ComputeAgentHash(string agentJson)
        {
            // agent JSON'u normalize edip (stable) SHA256 hash üretir (hex 64)
            var normalized = NormalizeJson(agentJson);
            var bytes = Encoding.UTF8.GetBytes(normalized);

            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(bytes);

            return ConvertToHex(hash); // 64 char
        }

        private static string NormalizeJson(string json)
        {
            // Amaç: whitespace, property order vb. farkları minimize etmek
            // JsonDocument -> JsonElement -> canonical string (sorted properties)
            using var doc = JsonDocument.Parse(json);
            var sb = new StringBuilder();
            WriteCanonical(doc.RootElement, sb);
            return sb.ToString();
        }

        private static void WriteCanonical(JsonElement el, StringBuilder sb)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    sb.Append('{');
                    bool first = true;

                    // property'leri isme göre sırala (canonical)
                    foreach (var prop in el.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal))
                    {
                        if (!first) sb.Append(',');
                        first = false;

                        sb.Append('\"').Append(Escape(prop.Name)).Append('\"').Append(':');
                        WriteCanonical(prop.Value, sb);
                    }

                    sb.Append('}');
                    break;

                case JsonValueKind.Array:
                    sb.Append('[');
                    bool firstArr = true;
                    foreach (var item in el.EnumerateArray())
                    {
                        if (!firstArr) sb.Append(',');
                        firstArr = false;
                        WriteCanonical(item, sb);
                    }
                    sb.Append(']');
                    break;

                case JsonValueKind.String:
                    sb.Append('\"').Append(Escape(el.GetString() ?? "")).Append('\"');
                    break;

                case JsonValueKind.Number:
                    // raw text kullan (1, 1.0 vs farkı minimal kalır)
                    sb.Append(el.GetRawText());
                    break;

                case JsonValueKind.True:
                    sb.Append("true");
                    break;
                case JsonValueKind.False:
                    sb.Append("false");
                    break;
                case JsonValueKind.Null:
                default:
                    sb.Append("null");
                    break;
            }
        }

        private static string Escape(string s)
            => s.Replace("\\", "\\\\").Replace("\"", "\\\"");

        private static string ConvertToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
