using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json.Serialization;
using WebApiNC.Attributes;
using WebApiNC.Controllers;
using WebApiNC.Middlewares;

namespace WebApiNC
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers(x => x.Filters.Add(new ResponseFilterAttribute()));

      services.AddApiVersioning(x =>
      {
        x.DefaultApiVersion = new ApiVersion(1, 0);
        x.AssumeDefaultVersionWhenUnspecified = true;
        x.ReportApiVersions = true;
      });

      services
        .AddMvc()
        .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
      });

      services.AddTransient<Service2>();
      services.AddScoped<Service1>();

      services
        .AddIdentityServer()
        //.AddInMemoryClients(new List<Client>())
        .AddInMemoryClients(Config.Clients)
        //.AddInMemoryIdentityResources(new List<IdentityResource>())
        .AddInMemoryIdentityResources(Config.IdentityResources)
        //.AddInMemoryApiResources(new List<ApiResource>())
        .AddInMemoryApiResources(Config.ApiResources)
        .AddInMemoryApiScopes(Config.ApiScopes)
        .AddTestUsers(Config.Users)
        .AddDeveloperSigningCredential();

      services.AddMemoryCache();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
      }

      //app.UseHttpsRedirection();

      app.UseMiddleware<RequestCultureMiddleware>();

      app.UseRouting();

      app.UseIdentityServer();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });


      lifetime.ApplicationStarted.Register(app.ApplicationStartedListener);
      lifetime.ApplicationStopping.Register(app.ApplicationStoppingListener);
    }
  }

  public static class ApplicationBuilderExtensions
  {
    public static void ApplicationStartedListener(this IApplicationBuilder app)
    {

      var mc = app.ApplicationServices.GetService(typeof(IMemoryCache)) as IMemoryCache;
      //mc.Set("key1", new Demo1 { Message = new Random().Next(100, 999).ToString() });

      var options = new MemoryCacheEntryOptions();

      options.RegisterPostEvictionCallback(PostEvictionCallback);
      // the given callback will be fired after the cache entry is evicted/removed from the cache.

      mc.Set("key1", "value1", options);
      mc.Set("key1", "value2", options);

      Console.WriteLine(mc.Get("key1") as string ?? "null");
    }

    public static void PostEvictionCallback(object key, object value, EvictionReason reason, object state)
    {
      Console.WriteLine($"evicted: {key}, {value}, Reason: {reason}, state: {state}");
    }














    public static void ApplicationStoppingListener(this IApplicationBuilder app)
    {
    }
  }
}
