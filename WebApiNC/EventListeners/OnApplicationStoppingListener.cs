using Microsoft.AspNetCore.Builder;
using System;

namespace WebApiNC.EventListeners
{
  public static class OnApplicationStoppingListener
  {
    public static void ApplicationStoppingListener(this IApplicationBuilder app)
    {
      Console.WriteLine("Application stopping.");
    }
  }
}
