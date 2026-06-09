using MediatR;
using TaskForge.Domain.Enums;

namespace TaskForge.Application.Tasks.Commands.CreateTask
{
    public record CreateTaskCommand(
        Guid ProjectId,
        string Title,
        string? Description,
        TaskItemPriority? Priority) : IRequest<Guid>;
}
