using Scorm.Business.Adapters.Abstract;
using Scorm.Business.Security;
using Scorm.Entities;
using Scorm.Entities.Enums;
using Scorm.Repositories.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Adapters
{
    public class XapiRuntimeAdapter : ILearningRuntimeAdapter
    {
        public ContentStandard Standard => ContentStandard.Xapi;

        private readonly string _xapiEndpointBase = "https://localhost:7269/xapi";
        public LaunchContext BuildLaunchContext(ContentPackage package, ContentAttempt attempt, ScormUserDto user)
        {
            var baseLaunchUrl =$"{package.FolderPath.TrimEnd('/')}/{package.LaunchPath.TrimStart('/')}";

            var actor = new
            {
                name = $"{user.FirstName} {user.LastName}",
                mbox = $"mailto:{user.Email}"
            };

            var actorJson = System.Text.Json.JsonSerializer.Serialize(actor);
            var activityId = BuildActivityId(package, attempt);
            var registration = attempt.Id;

            var queryParameters = new Dictionary<string, string>
            {
                ["endpoint"] = $"{_xapiEndpointBase.TrimEnd('/')}/",
                ["actor"] = actorJson, 
                ["activity_id"] = activityId,
                ["registration"] = registration.ToString(),
                ["auth"] = BasicAuthHandler.GetXapiKeySecret(),                  
                //["version"] = "1.0.3"
            };

            // 6) Querystring’i doğru şekilde üret
            // Not: Uri.EscapeDataString ile key/value encode ediyoruz
            var queryString = string.Join("&",
                queryParameters.Select(kvp =>
                    $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            var fullLaunchUrl = $"{baseLaunchUrl}?{queryString}";

            return new LaunchContext
            {
                LaunchUrl = fullLaunchUrl,
                QueryParameters = queryParameters,
                ClientConfig = new Dictionary<string, object>
                {
                    ["attemptId"] = attempt.Id,
                    ["userId"] = user.Id,
                    ["standard"] = "xapi"
                }
            };


            #region Old
            //var launchUrl = $"{package.FolderPath.TrimEnd('/')}/{package.LaunchPath.TrimStart('/')}";

            //var actor = new
            //{
            //    name = $"{user.FirstName} {user.LastName}",
            //    mbox = $"mailto:{user.Email}"
            //};

            //var actorJson = System.Text.Json.JsonSerializer.Serialize(actor);

            //var activityId = BuildActivityId(package, attempt);

            //var ctx = new LaunchContext
            //{
            //    LaunchUrl = launchUrl,
            //    QueryParameters = new Dictionary<string, string>
            //    {
            //        ["endpoint"] = $"{_xapiEndpointBase.TrimEnd('/')}/",
            //        ["actor"] = System.Text.Json.JsonSerializer.Serialize(actor),
            //        ["registration"] = attempt.Id.ToString()
            //        // auth'u da buraya gömebilirsin ya da cookie/token'la çözersin
            //    },
            //    ClientConfig = new Dictionary<string, object>
            //    {
            //        ["attemptId"] = attempt.Id,
            //        ["userId"] = user.Id,
            //        ["standard"] = "xapi"
            //    }
            //};

            //return ctx; 
            #endregion
        }

        private string BuildActivityId(ContentPackage package, ContentAttempt attempt)
        {
            return $"https://{package.FolderPath}/xapi/activities/" +
                   $"packages/{package.Id}/attempts/{attempt.Id}";
        }
    }
}
