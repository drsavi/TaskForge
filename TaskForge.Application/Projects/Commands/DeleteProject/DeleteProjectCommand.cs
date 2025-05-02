using MediatR;

namespace TaskForge.Application.Projects.Commands.DeleteProject
{
    public record DeleteProjectCommand(Guid Id) : IRequest<bool>;
}