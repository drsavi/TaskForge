using MediatR;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;

namespace TaskForge.Application.Projects.Queries.GetAllProjects
{
    public class GetAllProjectsHandler : IRequestHandler<GetAllProjectsQuery, IEnumerable<ProjectDto>>
    {
        private readonly IProjectRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public GetAllProjectsHandler(IProjectRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<ProjectDto>> Handle(GetAllProjectsQuery req, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllForOwnerAsync(_currentUser.UserId, cancellationToken);
            return entities.Select(e => new ProjectDto(e.Id, e.Name, e.Description));
        }
    }
}
