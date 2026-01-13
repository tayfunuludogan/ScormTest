using Scorm.Business.Adapters.Abstract;
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
    public class Scorm12RuntimeAdapter : ILearningRuntimeAdapter
    {
        public ContentStandard Standard => ContentStandard.Scorm12;

        public LaunchContext BuildLaunchContext(ContentPackage package, ContentAttempt attempt, ScormUserDto user)
        {
            var launchUrl = $"{package.FolderPath.TrimEnd('/')}/{package.LaunchPath.TrimStart('/')}";

            var ctx = new LaunchContext
            {
                LaunchUrl = launchUrl,
                ClientConfig = new Dictionary<string, object>
                {
                    ["attemptId"] = attempt.Id,
                    ["userId"] = user.Id,
                    ["standard"] = "scorm12",
                    ["serviceUrl"]= "https://localhost:7269/"
				}
            };

            return ctx;
        }

    }
}
