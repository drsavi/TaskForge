using MediatR;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Projects.Queries.GetAllProjects.TaskForge.Application.Projects.Queries.GetAllProjects;

namespace TaskForge.Application.Projects.Queries.GetAllProjects
{
    public class GetAllProjectsHandler : IRequestHandler<GetAllProjectsQuery, IEnumerable<ProjectDto>>
    {
        private readonly IProjectRepository _repository;
        public GetAllProjectsHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProjectDto>> Handle(GetAllProjectsQuery req, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);
            return entities.Select(e => new ProjectDto(e.Id, e.Name, e.Description));
        }
    }
}