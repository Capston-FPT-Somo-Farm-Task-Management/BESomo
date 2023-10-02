

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SomoTaskManagement.Notify.SubscribeTableDependencies;

namespace SomoTaskManagement.Notify.MiddlewareExtensions
{
    public static class ApplicationBuilderExtension
    {
        public static void UseSqlTableDependency<T>(this IApplicationBuilder applicationBuilder, string connectionString)
            where T : ISubscribeTableDependency
        {
            var serviceProvider = applicationBuilder.ApplicationServices;
            var service = serviceProvider.GetService<T>();
            service?.SubscribeTableDependency(connectionString);
        }
    }
}
