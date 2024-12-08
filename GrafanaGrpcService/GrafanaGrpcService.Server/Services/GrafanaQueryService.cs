using Grpc.Core;
using GrafanaGrpcService.Shared;
using GrafanaGrpcService.Server.DataCollector;

namespace GrafanaGrpcService.Server.Services;

public class GrafanaQueryService : GrafanaQueryAPI.GrafanaQueryAPIBase
{
    private readonly ILogger<GrafanaQueryService> _logger;
    private readonly DataCollectorService _dataCollector;

    public GrafanaQueryService(ILogger<GrafanaQueryService> logger, DataCollectorService dataCollector)
    {
        _logger = logger;
        _dataCollector = dataCollector;
    }

    public override Task<ListDimensionKeysResponse> ListDimensionKeys( ListDimensionKeysRequest request, ServerCallContext context )
    {
        var response = new ListDimensionKeysResponse();
        var dimensionKeys = _dataCollector.GetDimensionKeys(request.Filter);

        foreach( var key in dimensionKeys )
        {
            response.Results.Add( new ListDimensionKeysResponse.Types.Result
            {
                Key = key,
                Description = $"Description for {key}"
            } );
        }

        return Task.FromResult( response );
    }

    public override Task<ListDimensionValuesResponse> ListDimensionValues( ListDimensionValuesRequest request, ServerCallContext context )
    {
        var response = new ListDimensionValuesResponse();
        var dimensionValues = _dataCollector.GetDimensionValues(request.DimensionKey, request.Filter);

        foreach( var value in dimensionValues )
        {
            response.Results.Add( new ListDimensionValuesResponse.Types.Result
            {
                Value = value,
                Description = $"Description for {value}"
            } );
        }

        return Task.FromResult( response );
    }

    public override Task<GetMetricHistoryResponse> GetMetricHistory(GetMetricHistoryRequest request, ServerCallContext context)
    {
        var response = new GetMetricHistoryResponse();
        var data = _dataCollector.GetHistoricalData(request.Metric, request.StartDate, request.EndDate);

        foreach (var item in data)
        {
            response.Values.Add(new MetricHistoryValue
            {
                Timestamp = item.Timestamp,
                Value = new MetricValue { DoubleValue = item.Value }
            });
        }

        return Task.FromResult(response);
    }

    public override Task<ListMetricsResponse> ListMetrics( ListMetricsRequest request, ServerCallContext context )
    {
        var response = new ListMetricsResponse();
        var metrics = _dataCollector.GetAvailableMetrics(request.Dimensions, request.Filter);

        foreach( var metric in metrics )
        {
            response.Metrics.Add( new ListMetricsResponse.Types.Metric
            {
                Name = metric.Name,
                Description = metric.Description
            } );
        }

        return Task.FromResult( response );
    }

    public override Task<GetMetricValueResponse> GetMetricValue(GetMetricValueRequest request, ServerCallContext context)
    {
        var response = new GetMetricValueResponse
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Value = new MetricValue { DoubleValue = _dataCollector.GetLatestValue(request.Metric) }
        };
        return Task.FromResult(response);
    }


    public override Task<GetMetricAggregateResponse> GetMetricAggregate(GetMetricAggregateRequest request, ServerCallContext context)
    {
        var response = new GetMetricAggregateResponse();
        var aggregatedData = _dataCollector.GetAggregatedData(request.Metric, request.StartDate, request.EndDate, request.AggregateType, request.IntervalMs);

        foreach (var item in aggregatedData)
        {
            response.Values.Add(new MetricHistoryValue
            {
                Timestamp = item.Timestamp,
                Value = new MetricValue { DoubleValue = item.Value }
            });
        }

        return Task.FromResult(response);
    }    
}