using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SomoTaskManagement.Authentication;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Notify.HubSignalR;
using SomoTaskManagement.Notify.SubscribeTableDependencies;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Infrastructure.Configuration
{
    public static class ConfigurationService
    {
        public static void RegisterContextDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SomoTaskManagemnetContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MyDB"),
                    options => options.MigrationsAssembly("SomoTaskManagement.Data"));
            });
        }

        public static void RegisterDI (this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped<ITokenHandler, TokenHandler>();
            services.AddScoped<IMemberService,MemberService>();
            services.AddScoped<IMemberServiceToken, MemberTokenService>();
            services.AddScoped<IFarmService, FarmService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IZoneService, ZoneService>();
            services.AddScoped<IZoneTypeService, ZoneTypeService>();
            services.AddScoped<IFieldService, FieldService>();
            services.AddScoped<IPlantService, PlantService>();
            services.AddScoped<ILiveStockService, LiveStockService>();
            services.AddScoped<IHanbitantTypeService, HabitantTypeService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ITaskTypeService, TaskTypeService>();
            services.AddScoped<IOtherService, OtherService>();
            services.AddScoped<IMaterialService, MaterialService>();
            services.AddScoped<IEvidenceImageService, EvidenceImageService>();
            services.AddScoped<ITaskEvidenceService, TaskEvidenceService>();
            services.AddScoped<IFarmTaskService, FarmTaskService>();
            services.AddScoped<IRoleService, RoleService>();

            //services.AddScoped<TaskHub>();
            //services.AddScoped<ISubscribeTableDependency,SubscribeNotificationTableDependency>();

        }
    }
    
}
