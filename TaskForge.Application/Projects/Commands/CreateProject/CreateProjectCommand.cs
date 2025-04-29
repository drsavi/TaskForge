using MediatR;

namespace TaskForge.Application.Projects.Commands.CreateProject
{
    public record CreateProjectCommand(string Name, string? Description) : IRequest<Guid>;
}