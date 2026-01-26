using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scorm.Business.Adapters;
using Scorm.Business.Adapters.Abstract;
using Scorm.Business.Services;
using Scorm.Business.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {

            services.AddScoped<ILearningRuntimeAdapter, Scorm12RuntimeAdapter>();
            services.AddScoped<ILearningRuntimeAdapter, Scorm2004RuntimeAdapter>();
            services.AddScoped<ILearningRuntimeAdapter, XapiRuntimeAdapter>();
            services.AddScoped<ILearningRuntimeAdapterFactory, LearningRuntimeAdapterFactory>();
            
            services.AddScoped<IScormRuntimeService, Scorm12RuntimeService>();
            services.AddScoped<IScormRuntimeService, Scorm2004RuntimeService>();
            services.AddScoped<IScormRuntimeServiceFactory, ScormRuntimeServiceFactory>();
            services.AddScoped<IXapiRuntimeService, XapiRuntimeService>();

            services.AddScoped<IScormLearningService, ScormLearningService>();

            return services;
        }
    }
}
