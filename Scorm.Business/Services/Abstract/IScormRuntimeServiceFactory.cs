using Scorm.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Services.Abstract
{
    public interface IScormRuntimeServiceFactory
    {
        IScormRuntimeService GetService(ContentStandard standard);
    }
}
