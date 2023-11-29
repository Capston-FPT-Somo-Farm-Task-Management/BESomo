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
            services.AddScoped<IEmailService, EmailService>();
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

                var farmTaskJobKey = new JobKey(nameof(FarmTaskBackgroundJob));
                var subTaskJobKey = new JobKey(nameof(SubTaskBackGroundJob));

                options.AddJob<FarmTaskBackgroundJob>(farmTaskJobKey)
                    .AddTrigger(trigger => trigger
                        .ForJob(farmTaskJobKey)
                        .WithCronSchedule("0 0 0/12 * * ?"));

                options.AddJob<SubTaskBackGroundJob>(subTaskJobKey)
                    .AddTrigger(trigger => trigger
                        .ForJob(subTaskJobKey)
                        .WithCronSchedule("0 0 * * * ?"));

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


        public static void RegisterTwilio(this IServiceCollection services, IConfiguration configuration)
        {
            var twilioAccountSid = configuration["Twilio:AccountSid"];
            var twilioAuthToken = configuration["Twilio:AuthToken"];
            var twilioPhoneNumber = configuration["Twilio:PhoneNumber"];

            if (string.IsNullOrEmpty(twilioAccountSid) || string.IsNullOrEmpty(twilioAuthToken) || string.IsNullOrEmpty(twilioPhoneNumber))
            {
                throw new InvalidOperationException("Missing Twilio configuration");
            }

            services.AddSingleton(new TwilioService(twilioAccountSid, twilioAuthToken, twilioPhoneNumber));
        }

    }



}


