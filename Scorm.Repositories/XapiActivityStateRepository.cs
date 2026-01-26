using Microsoft.EntityFrameworkCore;
using Scorm.Core.Repositories;
using Scorm.Core.Utilities.Results;
using Scorm.Entities;
using Scorm.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Repositories
{
    public class XapiActivityStateRepository : RepositoryBase<XapiActivityState, LRSContext>, IXapiActivityStateRepository
    {
        public XapiActivityStateRepository(LRSContext lRSContext) : base(lRSContext)
        {

        }


        public async Task<IReadOnlyList<string>> ListStateIdsAsync(Guid attemptId, string activityId, string agentJson, string agentHash, CancellationToken ct = default)
        {
            return await Context.XapiActivityStates.AsNoTracking()
             .Where(x => x.AttemptId == attemptId && x.ActivityId == activityId && x.AgentHash == agentHash)
             .Select(x => x.StateId)
             .Distinct()
             .OrderBy(x => x)
             .ToListAsync(ct);
        }

    }
}
