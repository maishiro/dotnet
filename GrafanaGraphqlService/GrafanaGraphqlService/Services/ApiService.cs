using GrafanaGraphqlService.Types;
using Newtonsoft.Json.Linq;

namespace GrafanaGraphqlService.Services
{
    public class ApiService
    {
        private readonly ILogger<ApiService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public ApiService( ILogger<ApiService> logger, IConfiguration configuration )
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _apiUrl = configuration ["ApiSettings:ApiUrl"];
        }

        public async Task<MeasTemp> FetchDataAsync( DateTime start, DateTime end )
        {
            try
            {
                var response = await _httpClient.GetAsync( $"{_apiUrl}?start={start:yyyy-MM-ddTHH:mm:ss}&end={end:yyyy-MM-ddTHH:mm:ss}" );
                if( response.IsSuccessStatusCode )
                {
                    var timestamp = DateTime.Now;

                    var content = await response.Content.ReadAsStringAsync();

                    var data = JObject.Parse(content);
                    var value = data["temperatureC"].Value<double>();
                    return new MeasTemp { timestamp = timestamp, value = value };
                }
            }
            catch( Exception ex )
            {
                _logger.LogError( $"{ex.Message}", ex );
            }

            return new MeasTemp {
                timestamp = DateTime.Now,
                value = 0
            };
        }
    }
}
