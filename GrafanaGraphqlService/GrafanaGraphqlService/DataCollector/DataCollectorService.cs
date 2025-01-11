using GrafanaGraphqlService.Models.Enums;
using GrafanaGraphqlService.Models.Types;
using GrafanaGraphqlService.Services;
using GrafanaGraphqlService.Types;
using Newtonsoft.Json.Linq;

namespace GrafanaGraphqlService.DataCollector;

public class DataCollectorService : BackgroundService
{
    private readonly ILogger<DataCollectorService> _logger;
    private readonly ApiService _apiService;
    private readonly IConfiguration _configuration;
    private readonly List<(long Timestamp, JArray Value)> _collectedData = new();

    public DataCollectorService( ILogger<DataCollectorService> logger, ApiService apiService, IConfiguration configuration )
    {
        _logger = logger;
        _apiService = apiService;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CollectData();
                await Task.Delay(TimeSpan.FromMinutes(_configuration.GetValue<int>( "ApiSettings:DataCollectionIntervalMinutes", 5)), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while collecting data");
            }
        }
    }

    private async Task CollectData()
    {
        _logger.LogInformation( $"CollectData() start" );

        try
        {
            var interval = _configuration.GetValue<int>("ApiSettings:DataCollectionIntervalMinutes");
            var historyHours = _configuration.GetValue<int>("ApiSettings:HistoryHours");

            var now = DateTime.Now;
            var end = now.AddMinutes( -now.Minute % interval ).AddSeconds( -now.Second );
            var start = end.AddHours( -historyHours );

            var result = await _apiService.FetchDataAsync( start, end );
            if( result != null )
            {
                lock( _collectedData )
                {
                    _collectedData.Add( ( new DateTimeOffset(result.timestamp).ToUnixTimeSeconds(), result.values ) );
                }
            }
        }
        catch( Exception ex )
        {
            _logger.LogError( $"{ex.Message}", ex );
        }

        _logger.LogInformation( $"CollectData() end" );
    }

    // public IEnumerable<DimensionKey> GetDimensionKeys( string filter )
    public IEnumerable<string> GetDimensionKeys( string filter )
    {
        // ディメンションキーを取得する実装
        // サンプル 値
        var keys = new List<string> { "region", "service", "instance" };
        return string.IsNullOrEmpty( filter )
            ? keys
            : keys.Where( k => k.Contains( filter, StringComparison.OrdinalIgnoreCase ) );
    }

    public IEnumerable<string> GetDimensionValues( string dimensionKey, string filter )
    {
        // ディメンション値を取得する実装
        // サンプル 値
        var values = dimensionKey switch
        {
            "region" => new List<string> { "us-east-1", "us-west-2", "eu-central-1" },
            "service" => new List<string> { "web", "database", "cache" },
            "instance" => new List<string> { "i-1234567890abcdef0", "i-0987654321fedcba0" },
            _ => new List<string>()
        };

        return string.IsNullOrEmpty( filter )
            ? values
            : values.Where( v => v.Contains( filter, StringComparison.OrdinalIgnoreCase ) );
    }

    public IEnumerable<MeasTemp> GetHistoricalData(string metric, long startDate, long endDate)
    {
        lock (_collectedData)
        {
            return _collectedData
                .Where(d => d.Timestamp >= startDate && d.Timestamp <= endDate)
                .Select( d => new MeasTemp {
                    Timestamp = DateTimeOffset.FromUnixTimeSeconds( d.Timestamp ).DateTime,
                    Value = d.Value.Last()["temperatureC"].Value<float>()
                } )
                .ToList();
        }
    }

    public double GetLatestValue(string metric)
    {
        lock (_collectedData)
        {
            return _collectedData.LastOrDefault().Value.Last().Value<float>();
        }
    }

    public IEnumerable<MeasTemp> GetAggregatedData( string metric, long startDate, long endDate, AggregateType aggregateType, long intervalMs )
    {
        lock( _collectedData )
        {
            var data = _collectedData
                .Where(d => d.Timestamp >= startDate && d.Timestamp <= endDate)
                .Select( d => new MeasTemp
                {
                    Timestamp = DateTimeOffset.FromUnixTimeSeconds( d.Timestamp ).DateTime,
                    Value = d.Value.Last()["temperatureC"].Value<float>()
                } )
                .ToList();

            return AggregateData( data, aggregateType, intervalMs );
        }
    }

    private IEnumerable<MeasTemp> AggregateData( List<MeasTemp> data, AggregateType aggregateType, long intervalMs )
    {
        var groupedData = data
            .GroupBy(d => d.Timestamp.Ticks / (intervalMs / 1000))
            .Select(g => new
            {
                Timestamp = g.Key * (intervalMs / 1000),
                Values = g.Select(v => v.Value)
            });

        return aggregateType switch
        {
            AggregateType.Average => groupedData.Select( g => new MeasTemp { Timestamp = DateTimeOffset.FromUnixTimeSeconds( g.Timestamp ).DateTime, Value = g.Values.Average() } ),
            AggregateType.Max => groupedData.Select( g => new MeasTemp { Timestamp = DateTimeOffset.FromUnixTimeSeconds( g.Timestamp ).DateTime, Value = g.Values.Max() } ),
            AggregateType.Min => groupedData.Select( g => new MeasTemp { Timestamp = DateTimeOffset.FromUnixTimeSeconds( g.Timestamp ).DateTime, Value = g.Values.Min() } ),
            AggregateType.Count => groupedData.Select( g => new MeasTemp { Timestamp = DateTimeOffset.FromUnixTimeSeconds( g.Timestamp ).DateTime, Value = (double)g.Values.Count() } ),
            _ => throw new ArgumentException( "不正な集計タイプです", nameof( aggregateType ) )
        };
    }

    public IEnumerable<(string Name, string Description)> GetAvailableMetrics( IEnumerable<Dimension> dimensions, string filter )
    {
        // 利用可能なメトリクスを取得する
        var metrics = new List<(string Name, string Description)>
        {
            ("cpu_usage", "CPU使用率"),
            ("memory_usage", "メモリ使用率"),
            ("disk_io", "ディスクI/O"),
            ("network_in", "ネットワーク受信"),
            ("network_out", "ネットワーク送信")
        };

        // フィルタと次元に基づくメトリクスの絞り込み
        return metrics
            .Where( m => string.IsNullOrEmpty( filter ) || m.Name.Contains( filter, StringComparison.OrdinalIgnoreCase ) )
            .Where( m => FilterMetricsByDimensions( m.Name, dimensions ) );
    }

    private bool FilterMetricsByDimensions( string metricName, IEnumerable<Dimension> dimensions )
    {
        // とりあえず、すべてのメトリクスを許可
        return true;
    }
}