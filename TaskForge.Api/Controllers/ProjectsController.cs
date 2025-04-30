using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Application.Constants;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Application.Projects.Commands.CreateProject;
using TaskForge.Application.Projects.Queries.GetAllProjects.TaskForge.Application.Projects.Queries.GetAllProjects;
using TaskForge.Application.Projects.Queries.GetProjectById;

namespace TaskForge.Application.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _service;
        private readonly IMediator _mediator;

        public ProjectsController(IProjectService service, IMediator mediator)
        {
            _service = service;
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProjectDto>))]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll(CancellationToken cancellationToken)
        {
            var projects = await _mediator.Send(new GetAllProjectsQuery(), cancellationToken);
            return Ok(projects);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProjectDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationProblem(ModelState);

                var id = await _mediator.Send(new CreateProjectCommand(request.Name, request.Description));

                var dto = await _mediator.Send(new GetProjectByIdQuery(id));
                if (dto is null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new ProblemDetails
                        {
                            Title = "Project creation failed",
                            Detail = "An unexpected error occurred when retrieving the created project."
                        });
                }

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = dto.Id },
                    dto)
                ;
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return ValidationProblem(ModelState);
            }
        }

        [HttpGet(RouteConstants.Id)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _mediator.Send(new GetProjectByIdQuery(id), cancellationToken);
            if (dto is null) return NotFound();
            return Ok(dto);
        }

        [HttpPut(RouteConstants.Id)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto, CancellationToken cancellationtoken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            return await _service.UpdateAsync(id, dto, cancellationtoken)
                 ? NoContent()
                 : NotFound();
        }

        [HttpDelete(RouteConstants.Id)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationtoken)
        {
            return await _service.DeleteAsync(id, cancellationtoken)
                 ? NoContent()
                 : NotFound();
        }
    }
}