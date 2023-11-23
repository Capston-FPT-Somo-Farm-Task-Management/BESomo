using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace SomoTaskManagement.Socket
{
    public class WebSocketManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public void AddSocket(string key, WebSocket socket)
        {
            _sockets.TryAdd(key, socket);
        }

        public async Task RemoveSocket(string key)
        {
            _sockets.TryRemove(key, out var socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed by the WebSocketManager", CancellationToken.None);
        }

        public async Task SendToAll(string message)
        {
            foreach (var socket in _sockets)
            {
                if (socket.Value.State == WebSocketState.Open)
                {
                    await socket.Value.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
        public async Task SendEvidenceRealtime(string evidence)
        {
            foreach (var socket in _sockets)
            {
                if (socket.Value.State == WebSocketState.Open)
                {
                    try
                    {
                        var buffer = Encoding.UTF8.GetBytes(evidence);
                        await socket.Value.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
        }
    }

}
