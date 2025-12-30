using Scorm.Business.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Services.Abstract
{
    public interface IScorm12RuntimeService
    {
        Task<IDataResult<bool>> HandleCommitAsync(Guid attemptId, Dictionary<string,string> data); 
    }
}
