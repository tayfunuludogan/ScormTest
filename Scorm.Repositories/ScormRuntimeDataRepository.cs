using Microsoft.EntityFrameworkCore;
using Scorm.Core.Repositories;
using Scorm.Entities;
using Scorm.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Repositories
{
    public class ScormRuntimeDataRepository : RepositoryBase<ScormRuntimeData, LRSContext>, IScormRuntimeDataRepository
    {
        public ScormRuntimeDataRepository(LRSContext context) : base(context)
        {

        }

        public async Task<Dictionary<string, string>> GetRuntimeDataAsRowByAttemptId(Guid attemptId)
        {
            var rows = await Context.ScormRuntimeData
                        .AsNoTracking()
                        .Where(x => x.AttemptId == attemptId)
                        .Select(x => new { x.Element, x.Value })
                        .ToListAsync();

            var dict = rows.ToDictionary(x => x.Element, x => x.Value ?? "");
            return dict;

        }
    }
}
