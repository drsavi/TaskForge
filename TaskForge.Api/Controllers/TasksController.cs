using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Application.Dtos;
using TaskForge.Application.Tasks.Commands.CreateTask;
using TaskForge.Application.Tasks.Commands.DeleteTask;
using TaskForge.Application.Tasks.Commands.UpdateTask;
using TaskForge.Application.Tasks.Queries.GetTaskById;
using TaskForge.Application.Tasks.Queries.GetTasksByProject;
using TaskForge.Domain.Enums;

namespace TaskForge.Api.Controllers
{
    [Authorize]
    [Route("api/projects/{projectId:guid}/tasks")]
    public class TasksController(IMediator mediator) : ApiControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAll(
            Guid projectId,
            [FromQuery] TaskItemStatus? status,
            [FromQuery] TaskItemPriority? priority,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(
                new GetTasksByProjectQuery(projectId, status, priority),
                cancellationToken));
        }

        [HttpPost]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<TaskItemDto>> Create(
            Guid projectId,
            CreateTaskRequest req,
            CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(
                new CreateTaskCommand(projectId, req.Title, req.Description, req.Priority),
                cancellationToken);

            var dto = await _mediator.Send(new GetTaskByIdQuery(projectId, id), cancellationToken)
                ?? throw new KeyNotFoundException($"Task id '{id}' not found.");

            return CreatedAtAction(nameof(GetById), new { projectId, taskId = id }, dto);
        }

        [HttpGet("{taskId:guid}")]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<TaskItemDto>> GetById(
            Guid projectId,
            Guid taskId,
            CancellationToken cancellationToken)
        {
            var dto = await _mediator.Send(new GetTaskByIdQuery(projectId, taskId), cancellationToken);

            if (dto is null)
                throw new KeyNotFoundException($"Task id '{taskId}' not found.");

            return Ok(dto);
        }

        [HttpPut("{taskId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(
            Guid projectId,
            Guid taskId,
            UpdateTaskRequest req,
            CancellationToken cancellationToken)
        {
            var success = await _mediator.Send(
                new UpdateTaskCommand(
                    projectId,
                    taskId,
                    req.Title,
                    req.Description,
                    req.Priority,
                    req.Status,
                    req.DueDate),
                cancellationToken);

            if (!success)
                throw new KeyNotFoundException($"Task id '{taskId}' not found.");

            return NoContent();
        }

        [HttpDelete("{taskId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(
            Guid projectId,
            Guid taskId,
            CancellationToken cancellationToken)
        {
            var success = await _mediator.Send(new DeleteTaskCommand(projectId, taskId), cancellationToken);

            if (!success)
                throw new KeyNotFoundException($"Task id '{taskId}' not found.");

            return NoContent();
        }
    }
}
