using Scorm.Business.Adapters.Abstract;
using Scorm.Repositories.Dtos;
using Scorm.Repositories.Abstract;
using Scorm.Business.Services.Abstract;
using Scorm.Business.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Scorm.Business.Services
{
    public class ScormLearningService : IScormLearningService
    {
        private readonly IUserRepository _userRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ILearningRuntimeAdapterFactory _adapterFactory;
        public ScormLearningService(
            IUserRepository userRepository,
            IContentRepository contentRepository,
            ILearningRuntimeAdapterFactory adapterFactory)
        {
            _contentRepository = contentRepository;
            _userRepository = userRepository;
            _adapterFactory = adapterFactory;
        }

        public async Task<IDataResult<LaunchContext>> BuildLaunchContext(Guid packageId)
        {
            try
            {
                var user = await _userRepository.GetCurrentUserAsync();
                var package = await _contentRepository.GetPackageAsync(packageId);
                var attempt = await _contentRepository.CreateOrReuseAttemptAsync(packageId, user.Id);
                var adapter = _adapterFactory.GetAdapter(package.Standard); // Burada hangi scorm paketine göre çalışacak adapter i buluyoruz.
                var launchContext = adapter.BuildLaunchContext(package, attempt, user);
                return new SuccessDataResult<LaunchContext>(launchContext);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<LaunchContext>(ex.Message);
            }
        }
        public async Task<IDataResult<List<ContentPackageDto>>> GetPackages()
        {
            try
            {
                var data = new List<ContentPackageDto>();

                var packeges = await _contentRepository.GetPackagesAsync();
                if (packeges != null && packeges.Count > 0)
                {
                    data.AddRange(packeges.Select(x => new ContentPackageDto
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


    }
}
