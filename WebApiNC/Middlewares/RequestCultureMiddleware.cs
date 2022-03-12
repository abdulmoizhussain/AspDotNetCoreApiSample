using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace WebApiNC.Middlewares
{
  public class RequestCultureMiddleware
  {
    private readonly RequestDelegate _next, _nextWrapper;

    public RequestCultureMiddleware(RequestDelegate next, IConfiguration configuration)
    {
      _next = next;
      _nextWrapper = configuration.GetValue<bool>("TrapExceptions") ? TryNext : Next;
    }

    public Task InvokeAsync(HttpContext context)
    {
      // Call the next delegate/middleware in the pipeline.
      return _nextWrapper(context);
    }


    /// <summary>
    /// PRIVATE METHODS
    /// </summary>

    private async Task TryNext(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        await context.Response.WriteAsync(ex.Message);
      }
    }
    private Task Next(HttpContext context)
    {
      return _next(context);
    }
  }
}
