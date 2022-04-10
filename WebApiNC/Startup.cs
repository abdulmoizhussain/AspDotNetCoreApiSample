using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using WebApiNC.Attributes;
using WebApiNC.Controllers;
using WebApiNC.EventListeners;
using WebApiNC.Middlewares;

namespace WebApiNC
{
  public class Startup
  {
    private readonly IConfiguration Configuration;

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

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

      services.AddMvc().AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
      });

      services.AddTransient<Service2>();
      services.AddScoped<Service1>();

      services
        .AddIdentityServer()
        .AddInMemoryClients(Config.Clients)
        .AddInMemoryIdentityResources(Config.IdentityResources)
        .AddInMemoryApiResources(Config.ApiResources)
        .AddInMemoryApiScopes(Config.ApiScopes)
        .AddTestUsers(Config.Users)
        .AddSigningCredential(GetCertificate());


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

      app.UseMiddleware<RequestCultureMiddleware>();

      app.UseRouting();
      app.UseIdentityServer();
      app.UseAuthorization();

      app.UseEndpoints(endpoints => endpoints.MapControllers());

      lifetime.ApplicationStarted.Register(app.ApplicationStartedListener);
      lifetime.ApplicationStopping.Register(app.ApplicationStoppingListener);
    }

    private System.Security.Cryptography.X509Certificates.X509Certificate2 GetCertificate()
    {
      return new System.Security.Cryptography.X509Certificates.X509Certificate2("./JwtCertificate/rsa_cert.pfx", Configuration["JwtCertificate:Password"]);
    }
  }
}
