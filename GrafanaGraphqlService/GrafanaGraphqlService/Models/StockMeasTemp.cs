using Newtonsoft.Json.Linq;

namespace GrafanaGraphqlService.Models;

public class StockMeasTemp
{
    public DateTime timestamp { get; set; }

    public JArray values { get; set; }
}