using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SomoTaskManagement.Data.Abtract;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;

    public WebSocketMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
    {
        if (context.Request.Path == "/ws/notifications")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await HandleWebSocketConnection(webSocket, unitOfWork);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            await _next(context);
        }
    }

    private async Task HandleWebSocketConnection(WebSocket webSocket, IUnitOfWork unitOfWork)
    {
        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var notificationData = await unitOfWork.RepositoryNotifycation.GetData(null);
                //string notificationJson = JsonConvert.SerializeObject(notificationData);
                var notificationCount = notificationData.ToList().Count.ToString();
                var buffer = Encoding.UTF8.GetBytes(notificationCount);
                var segment = new ArraySegment<byte>(buffer);

                await webSocket.SendAsync(
                    segment,
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

                //await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Kết nối WebSocket đã đóng",
                    CancellationToken.None);
            }
        }
    }


}
