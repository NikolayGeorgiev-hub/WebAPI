using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Application.Common.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is BaseApplicationException exception)
        {
            context.HttpContext.Response.StatusCode = (int)exception.StatusCode;
        }
        else
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        ApplicationError error = context.Exception switch
        {
            BaseApplicationException ex => new ApplicationError(ex.Message, (int)ex.StatusCode),
            Exception ex => new ApplicationError("General error message.....", (int)HttpStatusCode.InternalServerError),
        };


        var response = new ResponseContent() { AppError = error };

        context.Result = new ObjectResult(response);
        context.ExceptionHandled = true;
    }
}
