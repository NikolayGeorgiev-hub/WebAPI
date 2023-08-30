using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text;
namespace Application.Common.Filters;

public class ModelStateFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {

    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        StringBuilder validationMessage = new();

        if (!context.ModelState.IsValid)
        {
            IEnumerable<IEnumerable<string>> messages = context.ModelState.Values.Select(x => x.Errors.Select(x => x.ErrorMessage));

            foreach (var message in messages)
            {
                validationMessage.Append(message);
            }

            ResponseContent response = new()
            {
                AppError = new ApplicationError(validationMessage.ToString(), (int)HttpStatusCode.BadRequest)
            };

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Result = new ObjectResult(response);
        }
    }
}
