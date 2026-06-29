using Microsoft.AspNetCore.Mvc;
using TaskForge.Api.Constants;

namespace TaskForge.Api.Controllers
{
    [ApiController]
    [Produces(MediaTypeConstants.Json)]
    [Consumes(MediaTypeConstants.Json)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, MediaTypeConstants.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, MediaTypeConstants.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden, MediaTypeConstants.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, MediaTypeConstants.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType, MediaTypeConstants.ProblemJson)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, MediaTypeConstants.ProblemJson)]
    public abstract class ApiControllerBase : ControllerBase
    {
    }
}
