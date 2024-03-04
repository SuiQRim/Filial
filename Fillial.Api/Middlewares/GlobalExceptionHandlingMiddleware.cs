using Microsoft.AspNetCore.Mvc;
using PrinterFil.Api.Exceptions;
using System.Net;
using System.Text.Json;

namespace PrinterFil.Api.Middlewares
{
	public class GlobalExceptionHandlingMiddleware : IMiddleware
	{
		private readonly ILogger _logger;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				await next(context);
			}
			catch (EntityNotFoundExceptions e)
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
				ProblemDetails problem = new()
				{
					Status = (int)HttpStatusCode.NotFound,
					Type = "Not Found",
					Title = "Not Found",
					Detail = e.Message
				};
				var json = JsonSerializer.Serialize(problem);
				context.Response.ContentType = "application/json";
				await context.Response.WriteAsJsonAsync(json);
			
			}
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

				ProblemDetails problem = new()
				{
					Status = (int)HttpStatusCode.InternalServerError,
					Type = "Server error",
					Title = "Server error",
					Detail = "An internal server has occurred"
				};

				var json = JsonSerializer.Serialize(problem);
				context.Response.ContentType = "application/json";
				await context.Response.WriteAsJsonAsync(json);

			}

		}
	}
}
