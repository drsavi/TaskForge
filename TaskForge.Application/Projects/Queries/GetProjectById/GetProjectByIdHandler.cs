using MediatR;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;

namespace TaskForge.Application.Projects.Queries.GetProjectById
{
    public class GetProjectByIdHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
    {
        private readonly IProjectRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public GetProjectByIdHandler(IProjectRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ProjectDto?> Handle(GetProjectByIdQuery req, CancellationToken cancellationToken)
        {
            var project = await _repository.GetByIdForOwnerAsync(req.Id, _currentUser.UserId, cancellationToken);
            return project is null
                ? null
                : new ProjectDto(project.Id, project.Name, project.Description);
        }
    }
}
