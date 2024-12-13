using GrafanaGraphqlService.DataCollector;

namespace GrafanaGraphqlService.Types;

public class Query
{
    private readonly ILogger<Query> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly DataCollectorService _dataCollector;
        
    public Query( ILogger<Query> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration, DataCollectorService dataCollector )
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _dataCollector = dataCollector;
    }

    public Book GetBook() =>
        new Book
        {
            Title = "C# in depth.",
            Author = new Author
            {
                Name = "Jon Skeet"
            }
        };

    public List<MeasTemp> GetMetricHistory() =>
        _dataCollector.GetHistoricalData("",0,2).Select( x => new MeasTemp() { timestamp = DateTimeOffset.FromUnixTimeSeconds( x.Timestamp ).DateTime, value = x.Value } ).ToList();

}