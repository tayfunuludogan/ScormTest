
using Scorm.Entities;
using Scorm.Entities.Enums;
using Scorm.Repositories.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Adapters.Abstract
{
    public interface ILearningRuntimeAdapter
    {
        ContentStandard Standard { get; }
        LaunchContext BuildLaunchContext(ContentPackage package, ContentAttempt attempt, ScormUserDto user);

    }
}
