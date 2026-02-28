using NLog;
using NLog.Config;
using NLog.Targets;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Collections.Generic;
using System.Diagnostics;

public class AspireDashboardFixture : IDisposable
{
    // テストプロジェクト固有のActivitySourceを定義
    public static readonly ActivitySource MyActivitySource = new("MyTestProject.Tests");
    private readonly TracerProvider? _tracerProvider;

    public AspireDashboardFixture()
    {
        var config = new LoggingConfiguration();

        // OpenTelemetry ターゲットの設定
        // Aspire ダッシュボードのデフォルト OTLP エンドポイント (gRPC) は http://localhost:4317
        var otlpTarget = new OtlpTarget
        {
            Name = "otlp",
            // Endpoint = "http://localhost:4317",
            // AppHost環境なら環境変数から取得、Docker等ならデフォルト値を指定
            Endpoint = Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL") ?? "http://localhost:19140",
            ServiceName = "xUnitTests",
        };

        config.AddTarget(otlpTarget);
        config.AddRuleForAllLevels(otlpTarget);

        // ローリングファイル出力設定
        var fileTarget = new FileTarget("logfile")
        {
            // 実行ディレクトリの logs フォルダに出力
            FileName = "${basedir}/logs/test.log",
            // ログのフォーマット
            Layout = "${date:format=yyyy-MM-ddTHH\\:mm\\:ss.ffff} ${level:uppercase=true} ${logger} ${message} ${exception:format=tostring}",
            // ローリング設定
            ArchiveFileName = "${basedir}/logs/archives/test-{#}.log",
            ArchiveEvery = FileArchivePeriod.Day,
            MaxArchiveFiles = 7, // 7日分保持
            KeepFileOpen = true,
            Encoding = System.Text.Encoding.UTF8
        };
        config.AddTarget(fileTarget);
        config.AddRuleForAllLevels(fileTarget);

        // NLogに反映
        LogManager.Configuration = config;

        // --- OpenTelemetry Traceの設定 ---
        _tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource(MyActivitySource.Name)
            .SetResourceBuilder(OpenTelemetry.Resources.ResourceBuilder.CreateDefault().AddService("xUnit-Tests"))
            .AddHttpClientInstrumentation() // これによりHttpClientのコールが自動的にトレースされる
            .AddOtlpExporter(opt => {
                opt.Endpoint = new Uri(Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL") ?? "http://localhost:19140");
                string? apiKey = Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_PRIMARY_API_KEY");
                if (!string.IsNullOrEmpty(apiKey)) opt.Headers = $"x-otlp-api-key={apiKey}";
            })
            .Build();
    }

    public void Dispose()
    {
        _tracerProvider?.Dispose();
        LogManager.Shutdown();
    }
}
