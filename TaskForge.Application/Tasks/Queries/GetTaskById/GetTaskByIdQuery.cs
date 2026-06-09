using MediatR;
using TaskForge.Application.Dtos;

namespace TaskForge.Application.Tasks.Queries.GetTaskById
{
    public record GetTaskByIdQuery(Guid ProjectId, Guid TaskId) : IRequest<TaskItemDto?>;
}
