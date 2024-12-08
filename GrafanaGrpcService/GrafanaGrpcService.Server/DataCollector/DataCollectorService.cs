using GrafanaGrpcService.Shared;
using Newtonsoft.Json.Linq;

namespace GrafanaGrpcService.Server.DataCollector;

public class DataCollectorService : BackgroundService
{
    private readonly ILogger<DataCollectorService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly List<(long Timestamp, double Value)> _collectedData = new();

    public DataCollectorService(ILogger<DataCollectorService> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CollectData();
                await Task.Delay(TimeSpan.FromMinutes(_configuration.GetValue<int>("DataCollectionIntervalMinutes", 5)), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while collecting data");
            }
        }
    }

    private async Task CollectData()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(_configuration["DataApiUrl"]);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        try
        {
            var data = JObject.Parse(content);

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var value = data["temperatureC"].Value<double>();

            lock (_collectedData)
            {
                _collectedData.Add((timestamp, value));
            }
        }
        catch( Exception ex )
        {
            _logger.LogError( $"{ex.Message}", ex );
        }
    }

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

    public IEnumerable<(long Timestamp, double Value)> GetHistoricalData(string metric, long startDate, long endDate)
    {
        lock (_collectedData)
        {
            return _collectedData
                .Where(d => d.Timestamp >= startDate && d.Timestamp <= endDate)
                .ToList();
        }
    }

    public double GetLatestValue(string metric)
    {
        lock (_collectedData)
        {
            return _collectedData.LastOrDefault().Value;
        }
    }

    public IEnumerable<(long Timestamp, double Value)> GetAggregatedData(string metric, long startDate, long endDate, AggregateType aggregateType, long intervalMs)
    {
        lock (_collectedData)
        {
            var data = _collectedData
                .Where(d => d.Timestamp >= startDate && d.Timestamp <= endDate)
                .ToList();

            return AggregateData(data, aggregateType, intervalMs);
        }
    }

    private IEnumerable<(long Timestamp, double Value)> AggregateData(List<(long Timestamp, double Value)> data, AggregateType aggregateType, long intervalMs)
    {
        var groupedData = data
            .GroupBy(d => d.Timestamp / (intervalMs / 1000))
            .Select(g => new
            {
                Timestamp = g.Key * (intervalMs / 1000),
                Values = g.Select(v => v.Value)
            });

        return aggregateType switch
        {
            AggregateType.Average => groupedData.Select(g => (g.Timestamp, g.Values.Average())),
            AggregateType.Max => groupedData.Select(g => (g.Timestamp, g.Values.Max())),
            AggregateType.Min => groupedData.Select(g => (g.Timestamp, g.Values.Min())),
            AggregateType.Count => groupedData.Select(g => (g.Timestamp, (double)g.Values.Count())),
            _ => throw new ArgumentException("不正な集計タイプです", nameof(aggregateType))
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