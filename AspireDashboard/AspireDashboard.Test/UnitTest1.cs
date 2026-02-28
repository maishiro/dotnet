using Microsoft.Extensions.Logging;
using NLog;
using System.Diagnostics.Metrics;

namespace AspireDashboard.Test;

[Collection("AspireDashboardCollection")]
public class UnitTest1
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    // Metrics
    private static readonly Meter MyMeter = new("MyCompany.MyProduct.MyLibrary", "1.0");
    private static readonly Counter<long> MyFruitCounter = MyMeter.CreateCounter<long>("MyFruitCounter", "fruit", "Counts fruit by name and color");

    [Fact]
    public void Test1()
    {
        // Metrics
        MyFruitCounter.Add(1, new("name", "apple"), new("color", "red"));
        MyFruitCounter.Add(2, new("name", "lemon"), new("color", "yellow"));

        // テスト全体のSpanを開始
        using var activity = AspireDashboardFixture.MyActivitySource.StartActivity("Test1");
                
        Logger.Info("Test1 started - sending to Aspire Dashboard!");

        // activity?.SetTag("http.status", (int)response.StatusCode);
        activity?.SetTag("http.status", 200);
    }
}
