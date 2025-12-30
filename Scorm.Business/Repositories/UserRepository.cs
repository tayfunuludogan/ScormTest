using Microsoft.EntityFrameworkCore;
using Scorm.Business.DTOs;
using Scorm.Business.Repositories.Abstract;
using Scorm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Repositories
{
    //Sonradan usercontext ile requestten bilgileri alan bir yapı yapılacak.
    public class UserRepository : IUserRepository
    {

        private readonly LRSContext _context;
        public UserRepository(LRSContext context)
        {
            _context = context;
        }

        public Guid UserId { get => new Guid("736130ef-0582-4380-b27d-6b8ccfeac979"); }

        public async Task<ScormUserDto> GetCurrentUserAsync()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            return new ScormUserDto
            {
                Id=user.Id,
                Email=user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
