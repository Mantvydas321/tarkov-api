using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Tarkov.API.Application.Exceptions;

namespace Tarkov.API.Infrastructure;

public class HttpGlobalExceptionFilter : IExceptionFilter
{
    private readonly IHostEnvironment _env;
    private readonly ILogger<HttpGlobalExceptionFilter> _logger;
    private readonly ProblemDetailsFactory _problemFactory;

    public HttpGlobalExceptionFilter(IHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger, ProblemDetailsFactory problemFactory)
    {
        _env = env;
        _logger = logger;
        _problemFactory = problemFactory;
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case BadRequestException: 
            {
                _logger.LogInformation(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);
                var problem = _problemFactory.CreateProblemDetails(context.HttpContext, (int)HttpStatusCode.BadRequest, "Bad Request", detail: context.Exception.Message);
                context.Result = new BadRequestObjectResult(problem);
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                break;
            }
            case NotFoundException:
            {
                _logger.LogInformation(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                break;
            }
            default:
            {
                _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);
                var problem = _problemFactory.CreateProblemDetails(context.HttpContext, (int)HttpStatusCode.InternalServerError, title: "Internal Server Error");
                if (_env.IsDevelopment())
                {
                    problem.Detail = context.Exception.Message;
                    problem.Extensions["exception"] = context.Exception.StackTrace;
                }
                context.Result = new InternalServerErrorObjectResult(problem);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
            }
        }

        context.ExceptionHandled = true;
    }
}