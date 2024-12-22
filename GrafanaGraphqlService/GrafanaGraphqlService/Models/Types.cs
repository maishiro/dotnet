namespace GrafanaGraphqlService.Models.Types;

public class Dimension
{
    public string Key { get; set; }
    public string Description { get; set; }
}

public class DimensionKey
{
    public string Key { get; set; }
    public string Description { get; set; }
}

public class DimensionValue
{
    public string Value { get; set; }
    public string Description { get; set; }
}

public class Metric
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class DimensionInput
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class MetricAggregate
{
    public IEnumerable<MetricHistoryValue> Values { get; set; }
    public string NextToken { get; set; }
}

public class MetricHistory
{
    public IEnumerable<MetricHistoryValue> Values { get; set; }
    public string NextToken { get; set; }
}

//public class MetricValue
//{
//    public int Timestamp { get; set; }
//    public float Value { get; set; }
//}

public class MetricHistoryValue
{
    public int Timestamp { get; set; }
    public float Value { get; set; }
}
