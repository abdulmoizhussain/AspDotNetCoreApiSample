
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using WebApiNC.ResponseModels;

namespace WebApiNC.Controllers
{
  public class Service2
  {
    private readonly IMemoryCache _memoryCache;

    public Service2(IMemoryCache memoryCache)
    {
      _memoryCache = memoryCache;
    }

    public string GetItem(string key)
    {
      return _memoryCache.Get<Demo1>(key).Message;
    }

    public string UpdateItem(string key)
    {
      var a = _memoryCache.Get<Demo1>(key);
      a.Message = new Random().Next(100, 999).ToString();
      return a.Message;
    }
  }
  public class Demo1
  {
    public bool Status { get; set; }
    public string Message { get; set; }
  }
  public class Service1
  {
    public string Uuid { get; set; } = Guid.NewGuid().ToString();
  }

  [ApiController]
  [Route("[controller]")]
  public class HomeController : ControllerBase
  {
    Service1 service;

    private Service2 Service2 { get; }

    public HomeController(Service1 service, Service2 service2)
    {
      this.service = service;
      Service2 = service2;
    }

    [HttpPost]
    public string Index(Model model)
    {
      return System.Text.Json.JsonSerializer.Serialize(model);
      return "no versioning attribute.";
    }

    [HttpGet]
    public IActionResult Get2(Enum1 enum1)
    {
      return ApiResponse.BadRequest("asdf");
    }

    [HttpGet("get")]
    public string Get()
    {
      return Service2.GetItem("key1");
    }

    [HttpGet("set")]
    public string Set()
    {
      return Service2.UpdateItem("key1");
    }

    public class Model
    {
      public Enum1 Enum1 { get; set; }
    }

    public enum Enum1
    {
      Item1,
      Item2,
      Item3,
    }
  }
}
