using Scorm.Business.Adapters.Abstract;
using Scorm.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Adapters.Abstract
{
    public class LearningRuntimeAdapterFactory : ILearningRuntimeAdapterFactory
    {
        private readonly IEnumerable<ILearningRuntimeAdapter> _adapters;

        public LearningRuntimeAdapterFactory(IEnumerable<ILearningRuntimeAdapter> adapters)
        {
            _adapters = adapters;
        }

        public ILearningRuntimeAdapter GetAdapter(ContentStandard standard)
        {
            var adapter = _adapters.FirstOrDefault(a => a.Standard == standard);
            if (adapter == null)
                throw new InvalidOperationException($"No adapter registered for {standard}");

            return adapter;
        }
    }
}
