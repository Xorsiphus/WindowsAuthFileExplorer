using System.ComponentModel.DataAnnotations;
using WindowsAuthFileExplorer.ExceptionMiddleware.Exceptions;

namespace WindowsAuthFileExplorer.ExceptionMiddleware;

public class ExceptionMiddlewareService : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            context.Request.EnableBuffering();
            await next(context);
        }
        catch (ValidationException e)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(e.Message);
        }
        catch (CustomException e)
        {
            context.Response.StatusCode = e.StatusCode;
            await context.Response.WriteAsync(e.Message);
        }
        catch (EntityNotFoundException e)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync(e.Message);
        }
        catch (BadHttpRequestException e)
        {
            context.Response.StatusCode = e.StatusCode;
            await context.Response.WriteAsync(e.Message);
        }
        catch (Exception e)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(e.Message);
        }
    }
}