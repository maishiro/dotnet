using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebSocketWindowsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendController : ControllerBase
    {
        private readonly WebSocketHandler _webSocketHandler;

        public SendController(WebSocketHandler webSocketHandler)
        {
            _webSocketHandler = webSocketHandler;
        }

        [HttpPost]
        [Consumes( "application/json" )]
        public async Task<IActionResult> Post( [FromBody] object message )
        {
            Console.WriteLine( $"Received message type: {message?.GetType()}" );

            var str = message.ToString() ?? "";
            var objJSON = JObject.Parse( str );
            str = objJSON.ToString( Formatting.None );
            await _webSocketHandler.SendNotification( str );

            // メッセージをエコーバック
            var response = new {
                status = "Message sent successfully",
                data = message
            };
            return Ok( response );
        }
    }
}