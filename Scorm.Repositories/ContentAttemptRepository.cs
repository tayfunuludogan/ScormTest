using Microsoft.EntityFrameworkCore;
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
    public class ContentAttemptRepository:RepositoryBase<ContentAttempt,LRSContext>, IContentAttemptRepository
    {
        IContentPackageRepository _contentPackageRepository;
        public ContentAttemptRepository(LRSContext context,
            IContentPackageRepository contentPackageRepository
            ) : base(context)
        {
            _contentPackageRepository = contentPackageRepository;
        }

        public async Task<ContentAttempt> CreateOrReuseAttemptAsync(Guid packageId, Guid userId)
        {
            var package = await _contentPackageRepository.GetAsync(x => x.Id == packageId);
            if (package == null)
                throw new InvalidOperationException($"Package not found: {packageId}");


            var existingInProgress = await Context.ContentAttempts
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.PackageId == packageId &&
                    x.Status == Entities.Enums.AttemptStatus.InProgress);

            if (existingInProgress != null)
                return existingInProgress;


            var lastAttemptNo = await Context.ContentAttempts
                .Where(x => x.UserId == userId && x.PackageId == packageId)
                .MaxAsync(x => (int?)x.AttemptNo) ?? 0;

            var newAttempt = new ContentAttempt
            {
                Id = Guid.NewGuid(),
                PackageId = package.Id,
                UserId = userId,
                Standard = package.Standard,
                Status = Entities.Enums.AttemptStatus.InProgress,
                AttemptNo = lastAttemptNo + 1,
                StartedAt = DateTime.UtcNow,
            };

            await Context.ContentAttempts.AddAsync(newAttempt);

            var affected = await Context.SaveChangesAsync();
            if (affected <= 0)
                throw new Exception("Kayıt başarısız.");

            return newAttempt;
        }
    }


}
