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
        private readonly IMyService _service;

        public HelloController( IMyService service ) => _service = service;

        /// <summary>
        /// 指定された名前に対して挨拶メッセージを返します
        /// </summary>
        /// <param name="name">挨拶する対象の名前</param>
        /// <returns>挨拶メッセージ</returns>
        [HttpGet, Route( "api/hello/{name}" )]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get( string name ) => Ok( _service.Greet( name ) );
    }
}
