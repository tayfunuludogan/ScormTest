using Scorm.Core.Repositories;
using Scorm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Repositories.Abstract
{
    public interface IContentAttemptRepository : IAsyncEntityRepository<ContentAttempt>, IEntityRepository<ContentAttempt>
    {
        Task<ContentAttempt> CreateOrReuseAttemptAsync(Guid packageId, Guid userId);
    }
}
