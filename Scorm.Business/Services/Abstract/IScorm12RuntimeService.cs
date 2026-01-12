using Scorm.Core.Utilities.Results;
using Scorm.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Services.Abstract
{
    public interface IScorm12RuntimeService
    {
        ContentStandard Standard { get; }
        Task<IResult> HandleCommitAsync(Guid attemptId, Dictionary<string,string> data);
        Task<IDataResult<Dictionary<string, string>>> GetStateAsync(Guid attemptId);
    }
}
