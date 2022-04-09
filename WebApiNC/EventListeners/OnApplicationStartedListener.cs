using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace WebApiNC.EventListeners
{
  public static class OnApplicationStartedListener
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

    private static void PostEvictionCallback(object key, object value, EvictionReason reason, object state)
    {
      Console.WriteLine($"evicted: {key}, {value}, Reason: {reason}, state: {state}");
    }
  }
}
