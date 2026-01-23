using Scorm.Business.Services.Abstract;
using Scorm.Core.Utilities.Results;
using Scorm.Entities;
using Scorm.Entities.Enums;
using Scorm.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Scorm.Business.Services
{
    public class Scorm2004RuntimeService : IScormRuntimeService
    {
        public ContentStandard Standard => ContentStandard.Scorm2004;

        private readonly IUserRepository _userRepository;
        private readonly IContentAttemptRepository _contentAttemptRepository;
        private readonly IScormRuntimeDataRepository _scormRuntimeDataRepository;
        private readonly IContentAttemptScormSummaryRepository _contentAttemptScormSummaryRepository;

        public Scorm2004RuntimeService(
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

                var attempt = await _contentAttemptRepository.GetAsync(x =>
                    x.Id == attemptId &&
                    x.UserId == user.Id &&
                    x.Standard == this.Standard);

                if (attempt == null)
                    return new ErrorResult("Oturum bulunamadı.");

                // 1) ScormRuntimeData upsert (AttemptId, Element) unique
                var keys = data.Keys.ToList();
                var existings = await _scormRuntimeDataRepository.GetListAsync(x =>
                    x.AttemptId == attemptId && keys.Contains(x.Element));

                var existingsDictionary = existings.Items
                    .ToDictionary(x => x.Element, StringComparer.OrdinalIgnoreCase);

                foreach (var kvp in data)
                {
                    if (existingsDictionary.TryGetValue(kvp.Key, out var row))
                    {
                        row.Value = kvp.Value;
                        row.UpdatedAt = now;

                        // Eğer repository tracking yapmıyorsa gerekebilir:
                        // _scormRuntimeDataRepository.Update(row);
                    }
                    else
                    {
                        var newRow = new Entities.ScormRuntimeData
                        {
                            AttemptId = attemptId,
                            Element = kvp.Key,
                            Value = kvp.Value,
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

                // ==== SCORM 2004 key mapping ====
                data.TryGetValue("cmi.location", out var location);
                data.TryGetValue("cmi.completion_status", out var completionStatus);
                data.TryGetValue("cmi.success_status", out var successStatus);
                data.TryGetValue("cmi.exit", out var exitMode);
                data.TryGetValue("cmi.suspend_data", out var suspendData);
				data.TryGetValue("cmi.core.score.raw", out var scoreRawStr);
				//data.TryGetValue("cmi.score.raw", out var scoreRawStr);
                data.TryGetValue("cmi.score.min", out var scoreMinStr);
                data.TryGetValue("cmi.score.max", out var scoreMaxStr);
                data.TryGetValue("cmi.score.scaled", out var scoreScaledStr);

                data.TryGetValue("cmi.session_time", out var sessionTime);
                data.TryGetValue("cmi.total_time", out var totalTime);

                summary.LastLocation = location ?? summary.LastLocation;
                summary.RawCompletionStatus = completionStatus ?? summary.RawCompletionStatus;
                summary.RawSuccessStatus = successStatus ?? summary.RawSuccessStatus;
                summary.RawExitMode = exitMode ?? summary.RawExitMode;

                if (suspendData != null)
                    summary.SuspendData = suspendData;

                summary.SessionTime = sessionTime ?? summary.SessionTime;
                summary.TotalTime = totalTime ?? summary.TotalTime;

                if (TryParseDecimal(scoreRawStr, out var scoreRaw))
                    summary.ScoreRaw = scoreRaw;

                if (TryParseDecimal(scoreMinStr, out var scoreMin))
                    summary.ScoreMin = scoreMin;

                if (TryParseDecimal(scoreMaxStr, out var scoreMax))
                    summary.ScoreMax = scoreMax;

                if (TryParseDecimal(scoreScaledStr, out var scoreScaled))
                    summary.ScoreScaled = scoreScaled; // 0..1

                summary.LastCommitAt = now;

                // Attempt.Score normalize (0..100 recommended)
                if (summary.ScoreRaw.HasValue)
                {
                    attempt.Score = summary.ScoreRaw;
                }
                else if (summary.ScoreScaled.HasValue)
                {
                    attempt.Score = summary.ScoreScaled.Value * 100m;
                }

                // ====== Attempt lifecycle (EKLENDI) ======
                var mappedStatus = MapScorm2004Status(
                    summary.RawCompletionStatus,
                    summary.RawSuccessStatus,
                    summary.RawExitMode);

                attempt.Status = mappedStatus;

                attempt.LastActivityAt = now;

                if (attempt.Status == AttemptStatus.Completed ||
                    attempt.Status == AttemptStatus.Passed ||
                    attempt.Status == AttemptStatus.Failed ||
                    attempt.Status == AttemptStatus.Abandoned)
                {
                    attempt.FinishedAt = attempt.FinishedAt ?? now;
                }
                // ====== Attempt lifecycle (EKLENDI) ======

                // Eğer attempt tracking yoksa gerekebilir:
                _contentAttemptRepository.Update(attempt);
                _contentAttemptScormSummaryRepository.Update(summary);

                return new SuccessResult();
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex.Message);
            }
        }

        public async Task<IDataResult<Dictionary<string, string>>> GetStateAsync(Guid attemptId)
        {
            try
            {
                var user = await _userRepository.GetCurrentUserAsync();
                var attempt = await _contentAttemptRepository.GetAsync(x =>
                    x.Id == attemptId &&
                    x.UserId == user.Id &&
                    x.Standard == this.Standard);

                if (attempt == null)
                    return new ErrorDataResult<Dictionary<string, string>>("Oturum bulunamadı.");

                var dataRows = await _scormRuntimeDataRepository.GetRuntimeDataAsRowByAttemptId(attemptId);

                return new SuccessDataResult<Dictionary<string, string>>(dataRows);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Dictionary<string, string>>(ex.Message);
            }
        }

        private static bool TryParseDecimal(string? input, out decimal value)
        {
            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private static AttemptStatus MapScorm2004Status(string? completionStatus, string? successStatus, string? exitMode)
        {
            // exitMode bazı paketlerde "abandoned" gelebilir
            if (!string.IsNullOrWhiteSpace(exitMode) &&
                exitMode.Trim().Equals("abandoned", StringComparison.OrdinalIgnoreCase))
            {
                return AttemptStatus.Abandoned;
            }

            // success_status öncelikli (passed/failed/unknown)
            if (!string.IsNullOrWhiteSpace(successStatus))
            {
                var s = successStatus.Trim().ToLowerInvariant();
                if (s == "passed") return AttemptStatus.Passed;
                if (s == "failed") return AttemptStatus.Failed;
            }

            // completion_status ikinci (completed/incomplete/not attempted/unknown)
            if (!string.IsNullOrWhiteSpace(completionStatus))
            {
                var c = completionStatus.Trim().ToLowerInvariant();
                if (c == "completed") return AttemptStatus.Completed;
                return AttemptStatus.InProgress;
            }

            return AttemptStatus.InProgress;
        }
    }
}
