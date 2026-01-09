
using Scorm.Business.Services.Abstract;
using Scorm.Core.Utilities.Results;
using Scorm.Entities;
using Scorm.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Scorm.Business.Services
{
    public class Scorm12RuntimeService : IScorm12RuntimeService
    {
        private readonly IUserRepository _userRepository;
        private readonly IContentRepository _contentrepository;
        public Scorm12RuntimeService(
            IUserRepository userRepository,
            IContentRepository contentrepository)
        {
            _contentrepository = contentrepository;
            _userRepository = userRepository;
        }

        public async Task<IDataResult<bool>> HandleCommitAsync(Guid attemptId, Dictionary<string, string> data)
        {
            var now = DateTime.Now;
            var user = await _userRepository.GetCurrentUserAsync();

            var attempt = _contentrepository.GetAttemptById(attemptId, user.Id);
            if (attempt == null)
                return new ErrorDataResult<bool>("Oturum bulunamadı.");


            //1) ScormRuntimeData upsert(AttemptId, Element) unique
            var keys = data.Keys.ToList();

            var existings = await _contentrepository.GetExistingScormRuntimeData(attemptId, keys);

            var existingsDictionary = existings.ToDictionary(x => x.Element, StringComparer.OrdinalIgnoreCase);

            foreach (var keyValuePair in data)
            {
                if(existingsDictionary.TryGetValue(keyValuePair.Key,out var row))
                {
                    row.Value = keyValuePair.Value;
                    row.UpdatedAt = now;
                }
                else
                {
                    var newRow = new Entities.ScormRuntimeData
                    {
                        AttemptId = attemptId,
                        Element = keyValuePair.Key,
                        Value = keyValuePair.Value,
                        UpdatedAt = now
                    };
                }
            }


            // 2) AttemptScormSummary upsert (AttemptId PK)




            return new SuccessDataResult<bool>(true);
        }

    }
}
