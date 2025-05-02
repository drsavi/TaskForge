using MediatR;

namespace TaskForge.Application.Projects.Commands.UpdateProject
{
    public record UpdateProjectCommand(Guid Id, string Name, string? Description) : IRequest<bool>;
}