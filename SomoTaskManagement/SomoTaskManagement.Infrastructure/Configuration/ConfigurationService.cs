using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using SomoTaskManagement.Authentication;
using SomoTaskManagement.Cache;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Model.Configuration;
using SomoTaskManagement.Infrastructure.Quart;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Impf;
using SomoTaskManagement.Services.Interface;
using StackExchange.Redis;
using SomoTaskManagement.Socket;

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

        public static void RegisterDI(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped<ITokenHandler, TokenHandler>();
            services.AddScoped<IMemberService, MemberService>();
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
            services.AddScoped<IMaterialService, MaterialService>();
            services.AddScoped<IEvidenceImageService, EvidenceImageService>();
            services.AddScoped<ITaskEvidenceService, TaskEvidenceService>();
            services.AddScoped<IFarmTaskService, FarmTaskService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISubTaskService, SubTaskService>();
            services.AddScoped<IHubConnection, HubConnectionService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddSingleton<WebSocketManager>();

        }

        public static void RegisterCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConfiguration = new RedisConfiguration();

            configuration.GetSection("RedisConfiguration").Bind(redisConfiguration);

            services.AddSingleton(redisConfiguration);

            if (!redisConfiguration.Enable)
                return;

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString));
            services.AddStackExchangeRedisCache(option => option.Configuration = redisConfiguration.ConnectionString);
            services.AddSingleton<ICacheService, CacheService>();
        }

        public static void RegisterQuartz(this IServiceCollection services)
        {
            services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();

                var jobKey = JobKey.Create(nameof(FarmTaskBackgroundJob));

                options.AddJob<FarmTaskBackgroundJob>(jobKey)
                        .AddTrigger(trigger => trigger.ForJob(jobKey).WithSimpleSchedule(schedule =>
                        {
                            schedule.WithIntervalInHours(1).RepeatForever();
                        }));
            });

            services.AddQuartzHostedService(option =>
            {
                option.WaitForJobsToComplete = true;
            });
        }
        public static void RegisterWebSocket(this IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.UseWebSocketManager("/ws/countEvidence");
        }

    }



}


