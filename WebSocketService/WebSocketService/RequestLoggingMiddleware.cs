namespace WebSocketService
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware( RequestDelegate next )
        {
            _next = next;
        }

        public async Task Invoke( HttpContext context )
        {
            var request = context.Request;
            var requestBodyStream = new MemoryStream();
            await request.Body.CopyToAsync( requestBodyStream );
            requestBodyStream.Seek( 0, SeekOrigin.Begin );

            var requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();
            Console.WriteLine( $"Received request body: {requestBodyText}" );

            requestBodyStream.Seek( 0, SeekOrigin.Begin );
            context.Request.Body = requestBodyStream;

            await _next( context );
        }
    }
}
