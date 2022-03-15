using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApiNC.ResponseModels
{
  public class ApiResponse
  {
    public object Data { get; set; }
    public string Message { get; set; }
    public IEnumerable<string> Errors { get; set; }


    public static BadRequestObjectResult BadRequest(string errorMessage)
    {
      return new(new ApiResponse { Errors = new[] { errorMessage } });
    }
  }
}
