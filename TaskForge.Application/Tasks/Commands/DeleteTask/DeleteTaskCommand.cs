using MediatR;

namespace TaskForge.Application.Tasks.Commands.DeleteTask
{
    public record DeleteTaskCommand(Guid ProjectId, Guid TaskId) : IRequest<bool>;
}
