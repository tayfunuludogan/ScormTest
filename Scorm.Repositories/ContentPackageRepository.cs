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
    public class ContentPackageRepository: RepositoryBase<ContentPackage,LRSContext>,IContentPackageRepository
    {
        public ContentPackageRepository(LRSContext context) : base(context)
        {
        }
    }
}
