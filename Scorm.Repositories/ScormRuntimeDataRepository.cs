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
    public class ScormRuntimeDataRepository:RepositoryBase<ScormRuntimeData,LRSContext>, IScormRuntimeDataRepository
    {
        public ScormRuntimeDataRepository(LRSContext context):base(context)
        {

        }
    }
}
