using Prometheus;
using System.Diagnostics.Metrics;
using System.Net;

const int MetricsPort = 8081;
var server = new MetricServer(MetricsPort);

try
{
    // On .NET Framework, starting the server requires either elevation to Administrator or permission configuration.
    server.Start();
}
catch (HttpListenerException ex)
{
    Console.WriteLine($"Failed to start metric server: {ex.Message}");
    Console.WriteLine("You may need to grant permissions to your user account if not running as Administrator:");
    Console.WriteLine($"netsh http add urlacl url=http://+:{MetricsPort}/metrics user=DOMAIN\\user");
    return;
}

// Doesn't work: You need the OpenTelemetry support to get Prometheus to see/expose that
var meter = new Meter("MWAM.AllocationProcessor");
var requests = meter.CreateCounter<int>("mwam.ap.requests.count");
var twapOrdersCount = meter.CreateUpDownCounter<int>("mwam.ap.orders.twap.count");

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
