using dfSelfHostWebAPI.Services;
using System.Web.Http;
using System.Web.Http.Description;

namespace dfSelfHostWebAPI.Controllers
{
    /// <summary>
    /// 挨拶を提供するAPI
    /// </summary>
    public class HelloController : ApiController
    {
        private readonly ILogger<HelloController> _logger;
        private readonly IMyService _service;

        public HelloController( ILogger<HelloController> logger, IMyService service )
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// 指定された名前に対して挨拶メッセージを返します
        /// </summary>
        /// <param name="name">挨拶する対象の名前</param>
        /// <returns>挨拶メッセージ</returns>
        [HttpGet, Route( "api/hello/{name}" )]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get( string name )
        {
            _logger.Info( $"Get called with name: {name}" );
            return Ok( _service.Greet( name ) );
        }
    }
}
