using Scorm.Core.Repositories;
using Scorm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Repositories.Abstract
{
    public interface IScormRuntimeDataRepository: IAsyncEntityRepository<ScormRuntimeData>, IEntityRepository<ScormRuntimeData>
    {

    }
}
