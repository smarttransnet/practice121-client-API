using System;
using System.Net.Sockets;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Web.Api.Infrastructure;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        bool isDatabaseError = exception is NpgsqlException
            || exception is SocketException
            || exception is TimeoutException
            || exception is DbUpdateException
            || exception.InnerException is NpgsqlException
            || exception.InnerException is SocketException;

        var problemDetails = new ProblemDetails
        {
            Status = isDatabaseError ? StatusCodes.Status503ServiceUnavailable : StatusCodes.Status500InternalServerError,
            Type = isDatabaseError ? "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.4" : "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = isDatabaseError ? "Database Connection Unavailable" : "Server failure",
            Detail = isDatabaseError
                ? "The server could not establish a connection to the PostgreSQL database. Please verify Cloud SQL instance state and network accessibility."
                : "An unexpected internal server error occurred."
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
