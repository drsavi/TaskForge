using MediatR;
using TaskForge.Application.Dtos;

namespace TaskForge.Application.Projects.Queries.GetProjectById
{
    public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto?>;
}