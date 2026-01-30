using BuildingBlocks.Exceptions;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlingMiddleware(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context); // Propusti request dalje u pipeline

            if (!context.Response.HasStarted &&
                context.Response.StatusCode >= 400 &&
                context.Response.ContentType != "application/json")
            {
                context.Response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Status = context.Response.StatusCode,
                    Detail = ReasonPhrases.GetReasonPhrase(context.Response.StatusCode),
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
        catch (Exception ex)
        {

            // Provera da li je response vec poceo
            if (context.Response.HasStarted)
            {
                throw;
            }

            var statusCode = ex is AppException appEx
                ? (int)appEx.StatusCode
                : StatusCodes.Status500InternalServerError;

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = ex.GetType().Name,
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            // U development modu dodaj stack trace za debugging
            if (_environment.IsDevelopment())
            {
                problemDetails.Extensions.Add("exception", ex.ToString());
                problemDetails.Extensions.Add("stackTrace", ex.StackTrace);
            }

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}