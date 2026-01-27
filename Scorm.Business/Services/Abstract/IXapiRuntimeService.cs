using Scorm.Business.Models;
using Scorm.Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Scorm.Business.Services.Abstract
{
    public interface IXapiRuntimeService
    {
        #region State
        Task<IResult> PutStateAsync(XapiStateRequest req, byte[] body, string contentType, string? ifMatch, CancellationToken ct);
        Task<IDataResult<XapiStateResponse?>> GetStateAsync(XapiStateRequest req, CancellationToken ct);
        Task<IResult> DeleteStateAsync(XapiStateRequest req, string? ifMatch, CancellationToken ct);
        Task<IDataResult<IReadOnlyList<string>>> ListStateIdsAsync(string activityId, string agentJson, Guid registration, CancellationToken ct);
        #endregion

        #region Statement
        Task<IReadOnlyList<Guid>> StoreAsync(JsonElement payload, CancellationToken ct);
        Task StoreWithIdAsync(Guid statementId, JsonElement statement, CancellationToken ct);
        #endregion


    }
}
