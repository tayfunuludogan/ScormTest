using Scorm.Core.Repositories;
using Scorm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Repositories.Abstract
{
    public interface IXapiActivityStateRepository : IAsyncEntityRepository<XapiActivityState>, IEntityRepository<XapiActivityState>
    {
        Task<IReadOnlyList<string>> ListStateIdsAsync(Guid attemptId, string activityId, string agentJson, string agentHash, CancellationToken ct = default);
    }
}
