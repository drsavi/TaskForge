using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Api.Constants;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Application.Projects.Commands.CreateProject;
using TaskForge.Application.Projects.Queries.GetAllProjects.TaskForge.Application.Projects.Queries.GetAllProjects;
using TaskForge.Application.Projects.Queries.GetProjectById;

namespace TaskForge.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController(IProjectService service, IMediator mediator) : ControllerBase
    {
        private readonly IProjectService _service = service;
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetAllProjectsQuery(), cancellationToken));
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Create(CreateProjectRequest req)
        {
            var id = await _mediator.Send(new CreateProjectCommand(req.Name, req.Description));
            var dto = await _mediator.Send(new GetProjectByIdQuery(id)) ?? throw new KeyNotFoundException();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpGet(RouteConstants.Id)]
        public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _mediator.Send(new GetProjectByIdQuery(id), cancellationToken);

            if (dto is null)
                throw new KeyNotFoundException($"Project with id '{id}' not found.");

            return Ok(dto);
        }

        [HttpPut(RouteConstants.Id)]
        public async Task<IActionResult> Update(Guid id, UpdateProjectDto dto, CancellationToken cancellationtoken)
        {
            var updated = await _service.UpdateAsync(id, dto, cancellationtoken);

            if (!updated)
                throw new KeyNotFoundException($"Project with id '{id}' not found.");

            return NoContent();
        }

        [HttpDelete(RouteConstants.Id)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);

            if (!deleted)
                throw new KeyNotFoundException($"Project with id '{id}' not found.");

            return NoContent();
        }
    }
}