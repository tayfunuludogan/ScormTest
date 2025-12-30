using Scorm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Repositories.Abstract
{
    public interface IContentRepository
    {

        Task<int> SaveChangesAsync();
        Task<List<ContentPackage>> GetPackagesAsync();
        Task<ContentPackage> GetPackageAsync(Guid packageId);


        Task<ContentAttempt> CreateOrReuseAttemptAsync(Guid packageId, Guid userId);
        Task<ContentAttempt> GetAttemptById(Guid attemptId, Guid userId);


        Task<List<ScormRuntimeData>> GetExistingScormRuntimeData(Guid attemptId, List<string> keys);
        Task<ScormRuntimeData> AddScormRuntimeData(ScormRuntimeData scormRuntimeData,bool isSave = true);

    }
}
