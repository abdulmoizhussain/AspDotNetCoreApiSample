using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using WebApiNC.ResponseModels;

namespace WebApiNC.Attributes
{
  public class ResponseFilterAttribute : ActionFilterAttribute
  {
    public override void OnResultExecuting(ResultExecutingContext context)
    {
      if (context.Result is Microsoft.AspNetCore.Mvc.BadRequestObjectResult badRequestObject)
      {
        if (badRequestObject.Value is Microsoft.AspNetCore.Mvc.ValidationProblemDetails validationDetails)
        {
          var errors = new List<string>();

          foreach (KeyValuePair<string, string[]> error in validationDetails.Errors)
          {
            foreach (string singleError in error.Value)
            {
              errors.Add(error.Key + ": " + singleError);
            }
          }

          var result = new ApiResponse { Errors = errors };

          context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(result);
        }
      }
      base.OnResultExecuting(context);
    }
  }
}
