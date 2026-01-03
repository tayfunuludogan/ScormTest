using Microsoft.EntityFrameworkCore;
using Scorm.Entities;
using Scorm.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly LRSContext _context;
        public ContentRepository(LRSContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<List<ContentPackage>> GetPackagesAsync()
        {
            return await _context.ContentPackages.OrderBy(x => x.Standard).ToListAsync();
        }
        public async Task<ContentPackage> GetPackageAsync(Guid packageId)
        {
            return await _context.ContentPackages.FirstOrDefaultAsync(x => x.Id == packageId);
        }


        public async Task<ContentAttempt> CreateOrReuseAttemptAsync(Guid packageId, Guid userId)
        {

            var package = await GetPackageAsync(packageId);
            if (package == null)
                throw new InvalidOperationException($"Package not found: {packageId}");


            var existingInProgress = await _context.ContentAttempts
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.PackageId == packageId &&
                    x.Status == Entities.Enums.AttemptStatus.InProgress);

            if (existingInProgress != null)
                return existingInProgress;


            var lastAttemptNo = await _context.ContentAttempts
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

            await _context.ContentAttempts.AddAsync(newAttempt);

            var affected = await _context.SaveChangesAsync();
            if (affected <= 0)
                throw new Exception("Kayıt başarısız.");

            return newAttempt;

        }
        public async Task<ContentAttempt> GetAttemptById(Guid attemptId, Guid userId)
        {
            var attempt = await _context.ContentAttempts.FirstOrDefaultAsync(x => x.Id == attemptId && x.UserId == userId);
            return attempt;
        }


        public async Task<List<ScormRuntimeData>> GetExistingScormRuntimeData(Guid attemptId, List<string> keys)
        {
            var existings = await _context.ScormRuntimeData.Where(x => x.AttemptId == attemptId && keys.Contains(x.Element)).ToListAsync();
            return existings;
        }

        public async Task<ScormRuntimeData> AddScormRuntimeData(ScormRuntimeData scormRuntimeData, bool isSave = true)
        {
            _context.ScormRuntimeData.Add(scormRuntimeData);
            if (isSave)
                await this.SaveChangesAsync();

            return scormRuntimeData;
        }
    }
}
