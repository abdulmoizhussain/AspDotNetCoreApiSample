using Microsoft.AspNetCore.Builder;
using System;

namespace WebApiNC.EventListeners
{
  public static class OnApplicationStoppingListener
  {
    public static void ApplicationStoppingListener(this IApplicationBuilder _)
    {
      Console.WriteLine("Application stopping.");
    }
  }
}
