using OpenTelemetry.Metrics;
using OpenTelemetry;
using System.Diagnostics.Metrics;

// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-collection#view-metrics-in-grafana-with-opentelemetry-and-prometheus

const int MetricsPort = 8081;
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .AddMeter("MWAM.AllocationProcessor")
    .AddPrometheusHttpListener(options => { options.UriPrefixes = [$"http://localhost:{MetricsPort}/"]; })
    .Build();

var meter = new Meter("MWAM.AllocationProcessor");

var requests = meter.CreateCounter<int>("mwam.ap.requests.count");
var twapOrdersCount = meter.CreateUpDownCounter<int>("mwam.ap.orders.twap.count");

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
