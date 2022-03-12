using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApiNC.Controllers
{
  public static class MemoryCacheCustomExtension
  {
    private class MemoryCacheData
    {
      public string Data { get; set; }
      public DateTimeOffset AbsoluteExpiration { get; set; }
    }
    public static void TrySetOrUpdate(this MemoryCache memoryCache, string key, string data, TimeSpan? slidingExpiration)
    {
      string customData = memoryCache.Get(key).ToString();

      var memoryCacheData = JsonConvert.DeserializeObject<MemoryCacheData>(customData);

      memoryCacheData.Data = data;

      memoryCache.Set(key, JsonConvert.SerializeObject(memoryCacheData), new MemoryCacheEntryOptions
      {
        SlidingExpiration = slidingExpiration,
        AbsoluteExpiration = memoryCacheData.AbsoluteExpiration,
      });
    }
  }


  [ApiController]
  [Route("v{version:apiVersion}/[controller]")]
  [Route("[controller]")]
  [ApiVersion("1")]
  [ApiVersion("2")]
  public class WeatherForecastController : ControllerBase
  {
    private static readonly MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
    private static readonly string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
      _logger = logger;
    }

    class Class1
    {
      public string Data { get; set; }
      public DateTimeOffset AbsoluteExpiration { get; set; }
    }



    [HttpGet("get")]
    [MapToApiVersion("2")]
    public IActionResult GetAV2(string version)
    {
      return Ok(new Class1 { Data = "version 2" });
      return Ok(DateTimeOffset.UtcNow.ToString());
    }

    [HttpGet("get")]
    public string GetA(string version)
    {
      return "version 1";
      var absoluteExpiration = DateTimeOffset.Now.AddSeconds(500);
      var memoryCacheEntryOptions = new MemoryCacheEntryOptions
      {
        AbsoluteExpiration = absoluteExpiration,
        SlidingExpiration = TimeSpan.FromSeconds(300),
      };
      var data = new Class1
      {
        Data = "some data",
        AbsoluteExpiration = absoluteExpiration,
      };
      var str = JsonConvert.SerializeObject(data);
      //_memoryCache.("key1", str, memoryCacheEntryOptions);
      _memoryCache.Set("key1", str, memoryCacheEntryOptions);

      return DateTime.UtcNow.ToString();
    }


    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
      //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
      //identity.AddClaim(new Claim(ClaimTypes.Name, ""));
      //var principal = new ClaimsPrincipal(identity);
      //JwtSecurityTokenHandler jwtSecurityTokenHandler  = new JwtSecurityTokenHandler();
      //jwtSecurityTokenHandler.ValidateToken();

      var rng = new Random();
      return Enumerable.Range(1, 5).Select(index => new WeatherForecast
      {
        Date = DateTime.Now.AddDays(index),
        TemperatureC = rng.Next(-20, 55),
        Summary = Summaries[rng.Next(Summaries.Length)]
      })
      .ToArray();
    }
  }
}
