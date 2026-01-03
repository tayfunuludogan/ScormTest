using Scorm.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Repositories.Dtos
{
    public class ContentPackageDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ContentStandard Standard { get; set; }
        public string StandartName
        {
            get
            {
                string name = string.Empty;
                switch (this.Standard)
                {
                    case ContentStandard.Scorm12: name = "Scorm12"; break;
                    case ContentStandard.Scorm2004: name = "Scorm2004"; break;
                    case ContentStandard.Xapi: name = "Xapi"; break;
                }
                return name;
            }
        }
    }
}
