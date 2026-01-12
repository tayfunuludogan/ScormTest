
using Scorm.Business.Services.Abstract;
using Scorm.Core.Utilities.Results;
using Scorm.Entities;
using Scorm.Entities.Enums;
using Scorm.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Scorm.Business.Services
{
    public class Scorm12RuntimeService : IScorm12RuntimeService
    {
        public ContentStandard Standard => ContentStandard.Scorm12;

        private readonly IUserRepository _userRepository;
        private readonly IContentAttemptRepository _contentAttemptRepository;
        private readonly IScormRuntimeDataRepository _scormRuntimeDataRepository;
        private readonly IContentAttemptScormSummaryRepository _contentAttemptScormSummaryRepository;


        public Scorm12RuntimeService(
            IUserRepository userRepository,
            IContentAttemptRepository contentAttemptRepository,
            IScormRuntimeDataRepository scormRuntimeDataRepository,
            IContentAttemptScormSummaryRepository contentAttemptScormSummaryRepository)
        {

            _contentAttemptRepository = contentAttemptRepository;
            _userRepository = userRepository;
            _scormRuntimeDataRepository = scormRuntimeDataRepository;
            _contentAttemptScormSummaryRepository = contentAttemptScormSummaryRepository;
        }

        public async Task<IResult> HandleCommitAsync(Guid attemptId, Dictionary<string, string> data)
        {
            try
            {
                var now = DateTime.Now;
                var user = await _userRepository.GetCurrentUserAsync();

                var attempt = await _contentAttemptRepository.GetAsync(x => x.Id == attemptId && x.UserId == user.Id && x.Standard == this.Standard);
                if (attempt == null)
                    return new ErrorDataResult<bool>("Oturum bulunamadı.");


                //1) ScormRuntimeData upsert(AttemptId, Element) unique
                var keys = data.Keys.ToList();
                var existings = await _scormRuntimeDataRepository.GetListAsync(x => x.AttemptId == attemptId && keys.Contains(x.Element));
                var existingsDictionary = existings.Items.ToDictionary(x => x.Element, StringComparer.OrdinalIgnoreCase);

                foreach (var keyValuePair in data)
                {
                    if (existingsDictionary.TryGetValue(keyValuePair.Key, out var row))
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

                        await _scormRuntimeDataRepository.AddAsync(newRow);
                    }
                }


                // 2) AttemptScormSummary upsert (AttemptId PK)
                var summary = await _contentAttemptScormSummaryRepository.GetAsync(x => x.AttemptId == attemptId);
                if (summary == null)
                {
                    summary = new ContentAttemptScormSummary { AttemptId = attemptId };
                    await _contentAttemptScormSummaryRepository.AddAsync(summary);
                }

                data.TryGetValue("cmi.core.lesson_status", out var lessonStatus);
                data.TryGetValue("cmi.core.lesson_location", out var lessonLocation);
                data.TryGetValue("cmi.core.exit", out var exitMode);
                data.TryGetValue("cmi.suspend_data", out var suspendData);
                data.TryGetValue("cmi.core.score.raw", out var scoreRawStr);
                data.TryGetValue("cmi.core.session_time", out var sessionTime);
                data.TryGetValue("cmi.core.total_time", out var totalTime);

                summary.RawLessonStatus = lessonStatus ?? summary.RawLessonStatus;
                summary.RawExitMode = exitMode ?? summary.RawExitMode;
                summary.LastLocation = lessonLocation ?? summary.LastLocation;
                summary.SessionTime = sessionTime ?? summary.SessionTime;
                summary.TotalTime = totalTime ?? summary.TotalTime;

                if (suspendData != null)
                    summary.SuspendData = suspendData;

                if (decimal.TryParse(scoreRawStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var scoreRaw))
                    summary.ScoreRaw = scoreRaw;

                summary.LastCommitAt = now;

                if (summary.ScoreRaw.HasValue)
                    attempt.Score = summary.ScoreRaw;


                // status map (SCORM 1.2)
                var mappedStatus = MapScorm12Status(lessonStatus);

                _contentAttemptScormSummaryRepository.Update(summary);

                return new SuccessResult();
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex.Message);
            }
        }


        private static AttemptStatus? MapScorm12Status(string? lessonStatus)
        {
            if (string.IsNullOrWhiteSpace(lessonStatus)) return null;

            switch (lessonStatus.Trim().ToLowerInvariant())
            {
                case "completed": return AttemptStatus.Completed;
                case "passed": return AttemptStatus.Passed;
                case "failed": return AttemptStatus.Failed;
                case "abandoned": return AttemptStatus.Abandoned; // bazı içerikler set edebilir
                case "incomplete":
                case "browsed":
                case "not attempted":
                    return AttemptStatus.InProgress;
                default:
                    return AttemptStatus.InProgress;
            }
        }

        public Task<IDataResult<Dictionary<string, string>>> GetStateAsync(Guid attemptId)
        {
            throw new NotImplementedException();
        }
    }

}
