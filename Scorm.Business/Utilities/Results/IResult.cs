using System;
using System.Collections.Generic;
using System.Text;

namespace Scorm.Business.Utilities.Results
{
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
    }
}
