using Scorm.Entities;
using Scorm.Repositories.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Repositories.Abstract
{
    public interface IUserRepository
    {
        Task<ScormUserDto> GetCurrentUserAsync();
    }
}
