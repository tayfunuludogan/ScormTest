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
    public class XapiStatementRepository:RepositoryBase<XapiStatement,LRSContext>, IXapiStatementRepository
    {
        public XapiStatementRepository(LRSContext context):base(context)
        {
            
        }
    }
}
