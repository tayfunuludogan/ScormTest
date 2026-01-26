using Microsoft.EntityFrameworkCore;
using Scorm.Business.Helpers;
using Scorm.Business.Models;
using Scorm.Business.Services.Abstract;
using Scorm.Core.Utilities.Results;
using Scorm.Entities;
using Scorm.Entities.Enums;
using Scorm.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Scorm.Business.Services
{
    public class XapiRuntimeService : IXapiRuntimeService
    {
        IContentAttemptRepository _contentAttemptRepository;
        IXapiActivityStateRepository _xapiActivityStateRepository;
        IXapiStatementRepository _xapiStatementRepository;
        IUserRepository _userRepository;

        public XapiRuntimeService(
            IContentAttemptRepository contentAttemptRepository
            , IXapiActivityStateRepository xapiActivityStateRepository
            , IXapiStatementRepository xapiStatementRepository
            , IUserRepository userRepository)
        {
            _contentAttemptRepository = contentAttemptRepository;
            _xapiActivityStateRepository = xapiActivityStateRepository;
            _xapiStatementRepository = xapiStatementRepository;
            _userRepository = userRepository;
        }


        public async Task<IResult> PutStateAsync(XapiStateRequest req, byte[] body, string contentType, string? ifMatch, CancellationToken ct)
        {
            try
            {
                var user = await _userRepository.GetCurrentUserAsync();

                // registration == attemptId
                var attempt = await ResolveAttemptAsync(user.Id, req.Registration, ct);

                if (attempt == null)
                    return new ErrorResult("Attempt not found.");
                //throw new XapiHttpException(404, "Attempt not found.");

                var agentHash = XapiAgentHashHelper.ComputeAgentHash(req.AgentJson);

                var row = await _xapiActivityStateRepository.GetAsync(x =>
                    x.AttemptId == attempt.Id &&
                    x.ActivityId == req.ActivityId &&
                    x.StateId == req.StateId &&
                    x.AgentHash == agentHash, null, ct);

                if (row == null)
                {
                    if (!string.IsNullOrWhiteSpace(ifMatch) && ifMatch.Trim() != "*")
                        return new ErrorResult("If-Match provided but state does not exist.");
                    //throw new XapiHttpException(412, "If-Match provided but state does not exist.");

                    row = new XapiActivityState
                    {
                        AttemptId = attempt.Id,
                        Registration = attempt.Id,
                        ActivityId = req.ActivityId,
                        StateId = req.StateId,
                        AgentHash = agentHash,
                        AgentJson = req.AgentJson,
                        ContentType = contentType,
                        Data = body,
                        ETag = XapiETagHelper.NewETag(),
                        UpdatedAt = DateTime.UtcNow
                    };

                    _xapiActivityStateRepository.Add(row);

                }
                else
                {
                    if (!XapiETagHelper.IfMatchSatisfied(ifMatch, row.ETag ?? "\"\""))
                        return new ErrorResult("ETag mismatch (If-Match failed).");
                    //throw new XapiHttpException(412, "ETag mismatch (If-Match failed).");

                    row.ContentType = contentType;
                    row.Data = body;
                    row.ETag = XapiETagHelper.NewETag();
                    row.UpdatedAt = DateTime.UtcNow;
                    row.AgentJson ??= req.AgentJson;

                    _xapiActivityStateRepository.Update(row);
                }

                attempt.LastActivityAt = DateTime.UtcNow;

                // summary: resume stateId set (ilk kez)
                attempt.XapiSummary ??= new ContentAttemptXapiSummary { AttemptId = attempt.Id };
                attempt.XapiSummary.ResumeStateId ??= req.StateId;
                //attempt.XapiSummary.Registration ??= attempt.Id;

                await _contentAttemptRepository.UpdateAsync(attempt);

                return new SuccessResult();
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex.Message);
            }
        }

        public async Task<IDataResult<XapiStateResponse?>> GetStateAsync(XapiStateRequest req, CancellationToken ct)
        {
            try
            {
                var user = await _userRepository.GetCurrentUserAsync();

                var attempt = await ResolveAttemptAsync(user.Id, req.Registration, ct);
                if (attempt == null) return null;

                var agentHash = XapiAgentHashHelper.ComputeAgentHash(req.AgentJson);

                var row = await _xapiActivityStateRepository.GetAsync(x =>
                    x.AttemptId == attempt.Id &&
                    x.ActivityId == req.ActivityId &&
                    x.StateId == req.StateId &&
                    x.AgentHash == agentHash, null, ct);

                if (row == null) return null;

                attempt.LastActivityAt = DateTime.UtcNow;
                await _contentAttemptRepository.UpdateAsync(attempt);


                var response = new XapiStateResponse
                {
                    Data = row.Data,
                    ContentType = string.IsNullOrWhiteSpace(row.ContentType) ? "application/octet-stream" : row.ContentType,
                    ETag = row.ETag ?? "\"\"",
                    UpdatedAtUtc = row.UpdatedAt
                };

                return new SuccessDataResult<XapiStateResponse?>(response);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<XapiStateResponse?>(ex.Message);
            }
        }


        public async Task<IResult> DeleteStateAsync(XapiStateRequest req, string? ifMatch, CancellationToken ct)
        {
            try
            {
                var user = await _userRepository.GetCurrentUserAsync();
                var attempt = await ResolveAttemptAsync(user.Id, req.Registration, ct);
                if (attempt == null)
                    return new ErrorResult("Attempt not found.");


                var agentHash = XapiAgentHashHelper.ComputeAgentHash(req.AgentJson);

                var row = await _xapiActivityStateRepository.GetAsync(x =>
                    x.AttemptId == attempt.Id &&
                    x.ActivityId == req.ActivityId &&
                    x.StateId == req.StateId &&
                    x.AgentHash == agentHash, null, ct);


                if (row == null)
                    return new ErrorResult("State not found.");

                if (!XapiETagHelper.IfMatchSatisfied(ifMatch, row.ETag ?? "\"\""))
                    return new ErrorResult("ETag mismatch (If-Match failed.");
                //throw new XapiHttpException(412, "ETag mismatch (If-Match failed).");

                _xapiActivityStateRepository.Delete(row);


                attempt.LastActivityAt = DateTime.UtcNow;
                await _contentAttemptRepository.UpdateAsync(attempt);
                return new SuccessResult();
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex.Message);
            }
        }


        public async Task<IDataResult<IReadOnlyList<string>>> ListStateIdsAsync(string activityId, string agentJson, Guid registration, CancellationToken ct)
        {
            var user = await _userRepository.GetCurrentUserAsync();

            var attempt = await ResolveAttemptAsync(user.Id, registration, ct);
            if (attempt == null) return new ErrorDataResult<IReadOnlyList<string>>(Array.Empty<string>());

            var agentHash = XapiAgentHashHelper.ComputeAgentHash(agentJson);

            var states = await _xapiActivityStateRepository.ListStateIdsAsync(attempt.Id, activityId, agentJson, agentHash, ct);

            return new SuccessDataResult<IReadOnlyList<string>>(states);

        }


        public async Task<IReadOnlyList<Guid>> StoreAsync(JsonElement payload, CancellationToken ct)
        {
            var user = await _userRepository.GetCurrentUserAsync();

            var normaliseResult = Normalize(payload);
            if (!normaliseResult.Success)
                return null;

            var list = normaliseResult.Data;

            var ids = new List<Guid>(list.Count);

            foreach (var stmtEl in list)
            {
                var statementId = ExtractOrCreateStatementId(stmtEl);
                ids.Add(statementId);

                var parsed = XapiStatementParser.Parse(stmtEl, statementId);

                // registration == attemptId bekliyoruz
                ContentAttempt attempt = null;
                if (parsed.Registration.HasValue)
                    attempt = await _contentAttemptRepository.GetAsync(x => x.UserId == user.Id && x.Id == parsed.Registration.Value, i => i.Include(c => c.XapiSummary), ct);



                // Insert raw statement
                await _xapiStatementRepository.AddAsync(new XapiStatement
                {
                    AttemptId = attempt.Id,
                    VerbId = parsed.VerbId,
                    ObjectId = parsed.ObjectId,
                    ActorJson = parsed.ActorJson,
                    ResultJson = parsed.ResultJson,
                    ContextJson = parsed.ContextJson,
                    RawJson = parsed.RawJson,
                    Timestamp = parsed.TimestampUtc,
                    StoredAt = DateTime.UtcNow
                });


                // Project to Attempt + Summary
                if (attempt != null)
                {
                    attempt.LastActivityAt = DateTime.UtcNow;

                    attempt.XapiSummary ??= new ContentAttemptXapiSummary { AttemptId = attempt.Id };
                    var xs = attempt.XapiSummary;

                    xs.ActorMbox ??= parsed.ActorMbox;
                    xs.ActorAccountHomePage ??= parsed.ActorAccountHomePage;
                    xs.ActorAccountName ??= parsed.ActorAccountName;
                    xs.LastStatementAt = parsed.TimestampUtc;

                    // score normalize
                    if (parsed.ScoreScaled.HasValue)
                    {
                        xs.ScoreScaled = parsed.ScoreScaled;
                        attempt.Score = parsed.ScoreScaled.Value * 100m;
                    }
                    else if (parsed.ScoreRaw.HasValue && parsed.ScoreMax.HasValue && parsed.ScoreMax.Value > 0)
                    {
                        xs.ScoreRaw = parsed.ScoreRaw;
                        xs.ScoreMax = parsed.ScoreMax;
                        attempt.Score = (parsed.ScoreRaw.Value / parsed.ScoreMax.Value) * 100m;
                    }

                    // status mapping
                    if (XapiVerbs.IsPassed(parsed.VerbId))
                    {
                        attempt.Status = AttemptStatus.Passed;
                        attempt.FinishedAt ??= parsed.TimestampUtc;
                        xs.CompletionVerbId = parsed.VerbId;
                        xs.CompletionAt ??= parsed.TimestampUtc;
                    }
                    else if (XapiVerbs.IsFailed(parsed.VerbId))
                    {
                        attempt.Status = AttemptStatus.Failed;
                        attempt.FinishedAt ??= parsed.TimestampUtc;
                        xs.CompletionVerbId = parsed.VerbId;
                        xs.CompletionAt ??= parsed.TimestampUtc;
                    }
                    else if (XapiVerbs.IsCompleted(parsed.VerbId))
                    {
                        attempt.Status = AttemptStatus.Completed;
                        attempt.FinishedAt ??= parsed.TimestampUtc;
                        xs.CompletionVerbId = parsed.VerbId;
                        xs.CompletionAt ??= parsed.TimestampUtc;
                    }

                    await _contentAttemptRepository.UpdateAsync(attempt);
                    
                }
            }

            //await _db.SaveChangesAsync(ct);
            return ids;
        }




        private async Task<ContentAttempt?> ResolveAttemptAsync(Guid userId, Guid attemptId, CancellationToken ct)
        {
            return await _contentAttemptRepository.GetAsync(a =>
             a.UserId == userId
             && a.Id == attemptId
             , x => x.Include(q => q.XapiSummary), ct);

        }

        private static IDataResult<List<JsonElement>> Normalize(JsonElement payload)
        {
            if (payload.ValueKind == JsonValueKind.Array)
                return new SuccessDataResult<List<JsonElement>>(payload.EnumerateArray().ToList());

            if (payload.ValueKind == JsonValueKind.Object)
                return new SuccessDataResult<List<JsonElement>>(new List<JsonElement> { payload });

            return new ErrorDataResult<List<JsonElement>>("Invalid statements payload.");

        }

        private static Guid ExtractOrCreateStatementId(JsonElement stmt)
        {
            if (stmt.TryGetProperty("id", out var idEl) && idEl.ValueKind == JsonValueKind.String)
                if (Guid.TryParse(idEl.GetString(), out var id)) return id;

            return Guid.NewGuid();
        }


       
    }
}
