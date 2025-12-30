using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Entities.Enums
{
    public enum AttemptStatus
    {
        InProgress=1,  //Devam Ediyor
        Completed=2,   //Tamamlandı
        Passed=3,      //Geçti
        Failed=4,      //Başarısız
        Abandoned=5    //Terk Edildi
    }
}
