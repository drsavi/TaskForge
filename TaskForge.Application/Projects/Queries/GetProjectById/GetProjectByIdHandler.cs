using MediatR;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Repositories;

namespace TaskForge.Application.Projects.Queries.GetProjectById
{
    public class GetProjectByIdHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
    {
        private readonly IProjectRepository _repository;

        public GetProjectByIdHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProjectDto?> Handle(GetProjectByIdQuery req, CancellationToken cancellationToken)
        {
            var project = await _repository.GetByIdAsync(req.Id, cancellationToken);
            return project is null
                ? null
                : new ProjectDto(project.Id, project.Name, project.Description);
        }
    }
}