using Microsoft.Extensions.Logging;
using NLog;

namespace AspireDashboard.Test;

[Collection("AspireDashboardCollection")]
public class UnitTest1
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
    [Fact]
    public void Test1()
    {
        // テスト全体のSpanを開始
        using var activity = AspireDashboardFixture.MyActivitySource.StartActivity("Test1");
                
        Logger.Info("Test1 started - sending to Aspire Dashboard!");

        // activity?.SetTag("http.status", (int)response.StatusCode);
        activity?.SetTag("http.status", 200);
    }
}
