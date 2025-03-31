using System.Reflection;
using GameOfLife.Presentation.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace GameOfLife.Presentation.ErrorHandling;

/// <summary>
/// Handles exceptions that occur during the processing of HTTP requests.
/// </summary>
public class ExceptionHandler : IExceptionHandler
{
    private static readonly string AssemblyName = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ArgumentOutOfRangeException argOutOfRange && argOutOfRange.Source == AssemblyName)
        {
            await BadRequest(argOutOfRange.Message).ExecuteAsync(httpContext);
            return true;
        }

        return false;
    }
}
