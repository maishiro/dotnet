using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebSocketWindowsService
{
    public class WebSocketHandler
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var connectId = Guid.NewGuid().ToString();

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var jsonMessage = JObject.Parse(message);

                        switch (jsonMessage["type"].ToString())
                        {
                            case "CONNECT":
                                await HandleConnect(webSocket, connectId);
                                break;
                            case "SETTING":
                                await HandleSetting(webSocket, connectId);
                                break;
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        _sockets.TryRemove(connectId, out _);
                    }
                }
            }
            catch (WebSocketException)
            {
                _sockets.TryRemove(connectId, out _);
            }
        }

        private async Task HandleConnect(WebSocket webSocket, string connectId)
        {
            var response = new
            {
                type = "CONNECT_ACK",
                connectId = connectId
            };

            await SendMessageAsync(webSocket, JsonConvert.SerializeObject(response));
            _sockets.TryAdd(connectId, webSocket);
        }

        private async Task HandleSetting(WebSocket webSocket, string connectId)
        {
            var response = new
            {
                type = "SETTING_ACK"
            };

            await SendMessageAsync(webSocket, JsonConvert.SerializeObject(response));
        }

        public async Task SendNotification(string message)
        {
            var notification = new
            {
                type = "NOTIFY",
                message = JsonConvert.DeserializeObject(message)
            };

            var jsonNotification = JsonConvert.SerializeObject(notification);

            foreach (var socket in _sockets)
            {
                await SendMessageAsync(socket.Value, jsonNotification);
            }
        }

        private async Task SendMessageAsync(WebSocket socket, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}