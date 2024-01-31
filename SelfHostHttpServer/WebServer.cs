using Microsoft.Owin.Hosting;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using Owin;

namespace SelfHostHttpServer;

public class WebServer
{
    private IDisposable _webapp;

    public void Start()
    {
        _webapp = WebApp.Start<Startup>("http://localhost:8080");
    }

    public void Stop()
    {
        _webapp?.Dispose();
    }
}

public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.Run(context =>
        {
            string t = DateTime.Now.Millisecond.ToString();
            return context.Response.WriteAsync(t + " Production OWIN App");
        });
    }
}