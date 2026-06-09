using MediatR;
using TaskForge.Application.Dtos;
using TaskForge.Domain.Enums;

namespace TaskForge.Application.Tasks.Queries.GetTasksByProject
{
    public record GetTasksByProjectQuery(
        Guid ProjectId,
        TaskItemStatus? Status,
        TaskItemPriority? Priority) : IRequest<IEnumerable<TaskItemDto>>;
}
