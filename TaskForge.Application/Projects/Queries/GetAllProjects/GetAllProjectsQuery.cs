using MediatR;
using TaskForge.Application.Dtos;

namespace TaskForge.Application.Projects.Queries.GetAllProjects
{
    public record GetAllProjectsQuery : IRequest<IEnumerable<ProjectDto>>;
}