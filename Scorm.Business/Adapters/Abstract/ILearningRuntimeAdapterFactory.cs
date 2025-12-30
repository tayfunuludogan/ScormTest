using Scorm.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Adapters.Abstract
{
    public interface ILearningRuntimeAdapterFactory
    {
        ILearningRuntimeAdapter GetAdapter(ContentStandard standard);
    }
}
