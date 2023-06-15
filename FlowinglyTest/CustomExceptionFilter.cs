namespace FlowinglyTest;

/// <summary>    /// Defines the <see cref="CustomExceptionFilter" />.
/// </summary>
public class CustomExceptionFilter : Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter
{
    private readonly Microsoft.Extensions.Logging.ILogger _logger;
    public CustomExceptionFilter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(typeof(CustomExceptionFilter));
    }

    public void OnException(ExceptionContext context)
    {
        int status=0;
        string message="";
        string messageCode = "";
        var exceptionType = context.Exception.GetType();
        if (exceptionType == typeof(XmlException))
        {
            message = "Xml Exception";
            status = (int)HttpStatusCode.BadRequest;
        }
        else if (exceptionType == typeof(ArgumentOutOfRangeException))
        {
            message = "Xml in the request email not in appropriate format";
            status = (int)HttpStatusCode.BadRequest;
        }
        else
        {
            message = context.Exception.Message;
            status = (int)System.Net.HttpStatusCode.InternalServerError;
        }

        context.ExceptionHandled = true;
        HttpResponse response = context.HttpContext.Response;
        response.StatusCode = status;
        response.ContentType = "application/json";
        var requestResponse = new ObjectResult(new ErrorDTO
        {
            StatusCode = status,
            Message = message,
            MessageCode = messageCode,
        });
        _ = response.WriteAsync(JsonConvert.SerializeObject(requestResponse));
        var err = message + " " + context.Exception.StackTrace + " \nInner :" + context.Exception.InnerException?.Message + " " + context.Exception.InnerException?.StackTrace;
        _logger.LogError(err, context.Exception);
    }
}

