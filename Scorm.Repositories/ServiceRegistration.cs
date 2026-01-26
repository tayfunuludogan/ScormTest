using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scorm.Entities;
using Scorm.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Repositories
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services,
                                                        IConfiguration configuration)
        {
            services.AddDbContext<LRSContext>(options =>options.UseSqlServer(configuration.GetConnectionString("LRSDataBase")));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IContentPackageRepository, ContentPackageRepository>();
            services.AddScoped<IContentAttemptRepository, ContentAttemptRepository>();
            services.AddScoped<IContentAttemptScormSummaryRepository, ContentAttemptScormSummaryRepository>();
            services.AddScoped<IScormRuntimeDataRepository, ScormRuntimeDataRepository>();
            services.AddScoped<IXapiActivityStateRepository, XapiActivityStateRepository>();
            services.AddScoped<IXapiStatementRepository, XapiStatementRepository>();



            return services;
        }
    }
}
