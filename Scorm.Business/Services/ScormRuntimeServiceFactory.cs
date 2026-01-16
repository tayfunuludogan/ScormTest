using Scorm.Business.Services.Abstract;
using Scorm.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Services
{
    public class ScormRuntimeServiceFactory : IScormRuntimeServiceFactory
    {
        private readonly IEnumerable<IScormRuntimeService> _services;
        public ScormRuntimeServiceFactory(IEnumerable<IScormRuntimeService> services)
        {
            _services = services;
        }

        public IScormRuntimeService GetService(ContentStandard standard)
        {
            var _service = _services.FirstOrDefault(s => s.Standard == standard);
            if (_service == null)
                throw new NotImplementedException($"Scorm runtime service for standard {standard} is not implemented.");

            return _service;
        }
    }
}
