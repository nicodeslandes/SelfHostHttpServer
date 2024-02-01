using Microsoft.Owin.Hosting;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using Owin;
using Microsoft.Owin.Builder;
using System.Web.Http;
using Prometheus;
using System.Collections.Generic;

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
        // Configure Web API for self-host. 
        HttpConfiguration config = new HttpConfiguration();
        config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );

        AspNetMetricServer.RegisterRoutes(config);

        app.UseWebApi(config); 
        
        app.Run(context =>
        {
            string t = DateTime.Now.Millisecond.ToString();
            return context.Response.WriteAsync(t + " Production OWIN App");
        });
    }
}

public class ValuesController : ApiController
{
    // GET api/values 
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/values/5 
    public string Get(int id)
    {
        return "value";
    }

    // POST api/values 
    public void Post([FromBody] string value)
    {
    }

    // PUT api/values/5 
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/values/5 
    public void Delete(int id)
    {
    }
}