using MediatR;
using TaskForge.Domain.Enums;

namespace TaskForge.Application.Tasks.Commands.UpdateTask
{
    public record UpdateTaskCommand(
        Guid ProjectId,
        Guid TaskId,
        string Title,
        string? Description,
        TaskItemPriority Priority,
        TaskItemStatus Status,
        DateTimeOffset? DueDate) : IRequest<bool>;
}
