using Microsoft.AspNetCore.Http;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPI.Security
{
    public static class BasicAuthHandler
    {
        private const string Key = "testkey";
        private const string Secret = "testsecret";

        public static bool IsAuthorized(HttpRequest request)
        {
            if (!request.Headers.TryGetValue("Authorization", out var header))
                return false;

            var value = header.ToString();
            if (!value.StartsWith("Basic "))
                return false;

            var encoded = value["Basic ".Length..];
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));

            return decoded == $"{Key}:{Secret}";
        }


        public static string GetXapiKeySecret()
        {
            var decoded = $"{Key}:{Secret}";
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(decoded));
            return $"Basic {encoded}";
        }
    }

}
