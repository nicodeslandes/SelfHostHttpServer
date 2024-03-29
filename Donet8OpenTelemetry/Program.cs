﻿using OpenTelemetry.Metrics;
using OpenTelemetry;
using System.Diagnostics.Metrics;
using OpenTelemetry.Exporter;

// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-collection#view-metrics-in-grafana-with-opentelemetry-and-prometheus


var meter = new Meter("MWAM.AllocationProcessor");

var requests = meter.CreateCounter<int>("mwam.ap.requests.count");
var twapOrdersCount = meter.CreateUpDownCounter<int>("mwam.ap.orders.twap.count");

const int MetricsPort = 8081;
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .AddMeter("MWAM.AllocationProcessor")
    .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
    {
        exporterOptions.Endpoint = new Uri("http://localhost:9090/api/v1/otlp/v1/metrics");
        exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
        metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
    })
    //.AddPrometheusHttpListene(options => { options.UriPrefixes = [$"http://localhost:{MetricsPort}/"]; })
    .AddConsoleExporter()
    .Build();


//var requests = Metrics.CreateCounter("mwam_ap_requests_count", "Request counts");
//var twapOrdersCount = Metrics.CreateGauge("mwam_ap_orders_twap_count", "Number of TWAP Orders");

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
            //Console.WriteLine("New operation: {0}", v);
            requests.Add(v);
            twapOrdersCount.Add(v - 5);
            //requests.Inc();
            //twapOrdersCount.Inc(v - 4);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Oooops: " + ex);
    }
});


Console.WriteLine("Press Enter to exit");
Console.ReadLine();
