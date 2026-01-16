using Scorm.Business.Adapters.Abstract;
using Scorm.Business.Services.Abstract;
using Scorm.Core.Utilities.Results;
using Scorm.Entities;
using Scorm.Repositories.Abstract;
using Scorm.Repositories.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Scorm.Business.Services
{
    public class ScormLearningService : IScormLearningService
    {
        private readonly IUserRepository _userRepository;
        private readonly IContentAttemptRepository _contentAttemptRepository;
        private readonly IContentPackageRepository _contentPackageRepository;
        private readonly ILearningRuntimeAdapterFactory _adapterFactory;
        private readonly IScormRuntimeServiceFactory _runtimeServiceFactory;
        public ScormLearningService(
            IUserRepository userRepository,
            IContentAttemptRepository contentAttemptRepository,
            IContentPackageRepository contentPackageRepository,
            ILearningRuntimeAdapterFactory adapterFactory,
            IScormRuntimeServiceFactory _runtimeServiceFactory)
        {
            _contentPackageRepository = contentPackageRepository;
            _contentAttemptRepository = contentAttemptRepository;
            _userRepository = userRepository;
            _adapterFactory = adapterFactory;
            _runtimeServiceFactory = _runtimeServiceFactory;
        }

        public async Task<IDataResult<List<ContentPackageDto>>> GetPackages()
        {
            try
            {
                var data = new List<ContentPackageDto>();
                var packeges = await _contentPackageRepository.GetListAsync(null, x => x.OrderBy(y => y.Standard), null, 0);

                if (packeges.Count > 0)
                {
                    data.AddRange(packeges.Items.Select(x => new ContentPackageDto
                    {
                        Id = x.Id,
                        Standard = x.Standard,
                        Title = x.Title
                    }).ToList());
                }

                return new SuccessDataResult<List<ContentPackageDto>>(data);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<ContentPackageDto>>(ex.Message);
            }
        }


        public async Task<IDataResult<LaunchContext>> BuildLaunchContext(Guid packageId)
        {
            try
            {
                var user = await _userRepository.GetCurrentUserAsync();
                var package = await _contentPackageRepository.GetAsync(x => x.Id == packageId);
                var attempt = await _contentAttemptRepository.CreateOrReuseAttemptAsync(packageId,user.Id);
                var adapter = _adapterFactory.GetAdapter(package.Standard);
                var launchContext = adapter.BuildLaunchContext(package, attempt, user);
                return new SuccessDataResult<LaunchContext>(launchContext);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<LaunchContext>(ex.Message);
            }
        }
        
        public async Task<IResult> HandleCommitAsync(Guid attemptId, Dictionary<string, string> data)
        {
            try
            {
                var attempt = await _contentAttemptRepository.GetAsync(x => x.Id == attemptId);
                var service = _runtimeServiceFactory.GetService(attempt.Standard);
                return await service.HandleCommitAsync(attemptId, data);
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
                var attempt = await _contentAttemptRepository.GetAsync(x => x.Id == attemptId);
                var service = _runtimeServiceFactory.GetService(attempt.Standard);
                return await service.GetStateAsync(attemptId);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Dictionary<string, string>>(ex.Message);
            }

        }

        
    }
}
