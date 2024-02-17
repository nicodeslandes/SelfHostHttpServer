using OpenTelemetry.Metrics;
using OpenTelemetry;
using System.Diagnostics.Metrics;
using OpenTelemetry.Exporter;
using System.Diagnostics;

// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-collection#view-metrics-in-grafana-with-opentelemetry-and-prometheus

const int MetricsPort = 8081;
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .AddMeter("MWAM.AllocationProcessor")
    //.AddPrometheusHttpListener(options => { options.UriPrefixes = [$"http://localhost:{MetricsPort}/"]; })
    .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
    {
        exporterOptions.Endpoint = new Uri("http://localhost:9090/api/v1/otlp/v1/metrics");
        exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
        metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
    })
    .Build();

var hostTags = new TagList { { "hostname", Environment.MachineName } };
var meterOptions = new MeterOptions("MWAM.AllocationProcessor")
{
    Tags = hostTags,
};
var meter = new Meter(meterOptions);

var requests = meter.CreateCounter<int>("mwam.ap.requests.count");
var twapOrdersCount = meter.CreateUpDownCounter<int>("mwam.ap.orders.twap.count");
string[] labels = ["EU", "US", "HK"];

_ = Task.Run(async () =>
{
    try
    {
        Console.WriteLine("Doing important work here");
        var rand = new Random();
        while (true)
        {
            var v = rand.Next(10);
            await Task.Delay(v);
            var tags = new TagList { { "region", labels[v % 3] } /*, { "host", Environment.MachineName } */};
            requests.Add(v);
            twapOrdersCount.Add(v - 5);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Oooops: " + ex);
    }
});


Console.WriteLine("Press Enter to exit");
Console.ReadLine();
