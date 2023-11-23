
using Microsoft.AspNetCore.Builder;

namespace SomoTaskManagement.Socket
{
    public static class WebSocketExtensions
    {
        public static IApplicationBuilder UseWebSocketManager(this IApplicationBuilder app, string path)
        {
            return app.Map(path, (x) => x.UseMiddleware<WebSocketMiddleware>());
        }
    }
}

