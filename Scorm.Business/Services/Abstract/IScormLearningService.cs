using Scorm.Repositories.Dtos;
using Scorm.Core.Utilities.Results;
using Scorm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Services.Abstract
{
    public interface IScormLearningService
    {
        Task<IDataResult<List<ContentPackageDto>>> GetPackages();
        Task<IDataResult<LaunchContext>> BuildLaunchContext(Guid packageId);
        Task<IResult> HandleCommitAsync(Guid attemptId, Dictionary<string, string> data);
        Task<IDataResult<Dictionary<string, string>>> GetStateAsync(Guid attemptId);
    }
}
