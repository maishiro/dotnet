using GrafanaGraphqlService.DataCollector;

namespace GrafanaGraphqlService.Types;

public class Query
{
    private readonly ILogger<Query> _logger;
    private readonly IConfiguration _configuration;
    private readonly DataCollectorService _dataCollector;
        
    public Query( ILogger<Query> logger, IConfiguration configuration, DataCollectorService dataCollector )
    {
        _logger = logger;
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

    [UseFiltering]
    public IQueryable<MeasTemp> GetMetricHistory( DateTime from, DateTime to ) =>
        _dataCollector
            .GetHistoricalData( "", new DateTimeOffset(from).ToUnixTimeSeconds(), new DateTimeOffset(to).ToUnixTimeSeconds() )
            //.Select( x => new MeasTemp() { timestamp = DateTimeOffset.FromUnixTimeSeconds( x.Timestamp ).DateTime, values = x.Value } )
            .AsQueryable();

}