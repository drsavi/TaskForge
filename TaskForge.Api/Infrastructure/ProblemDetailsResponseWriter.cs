using Microsoft.AspNetCore.Mvc;
using TaskForge.Api.Constants;

namespace TaskForge.Api.Infrastructure
{
    public static class ProblemDetailsResponseWriter
    {
        public static async Task WriteAsync(HttpContext context, int statusCode, ProblemDetails problem)
        {
            problem.Status ??= statusCode;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = MediaTypeConstants.ProblemJson;
            await context.Response.WriteAsJsonAsync(problem);
        }

        public static ObjectResult ProblemResult(int statusCode, ProblemDetails problem)
        {
            problem.Status ??= statusCode;

            return new ObjectResult(problem)
            {
                StatusCode = statusCode,
                ContentTypes = { MediaTypeConstants.ProblemJson }
            };
        }
    }
}
