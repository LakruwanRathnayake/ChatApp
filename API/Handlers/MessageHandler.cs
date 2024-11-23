// Handlers/WebSocketHandler.cs
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;

namespace API.Handlers
{
    public class WebSocketHandler
    {
        private readonly ConcurrentDictionary<string, WebSocket> _connectedClients = new();

        public void AddClient(string userId, WebSocket socket)
        {
            _connectedClients.TryAdd(userId, socket);
        }

        public void RemoveClient(string userId)
        {
            _connectedClients.TryRemove(userId, out _);
        }

        public async Task SendMessageToClientAsync(string userId, Message message)
        {
            if (_connectedClients.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
            {
                var messageJson = JsonSerializer.Serialize(message);
                var buffer = Encoding.UTF8.GetBytes(messageJson);
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
