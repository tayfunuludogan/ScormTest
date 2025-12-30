using Scorm.Business.DTOs;
using Scorm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Repositories.Abstract
{
    public interface IUserRepository
    {
        Task<ScormUserDto> GetCurrentUserAsync();
    }
}
