using MediatR;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Domain.Entities;

namespace TaskForge.Application.Projects.Commands.CreateProject
{
    public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, Guid>
    {
        private readonly IProjectRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public CreateProjectHandler(IProjectRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(CreateProjectCommand createProjectCommand, CancellationToken cancellationToken)
        {
            var project = new Project(
                Guid.NewGuid(),
                _currentUser.UserId,
                createProjectCommand.Name,
                createProjectCommand.Description);

            await _repository.AddAsync(project, cancellationToken);
            return project.Id;
        }
    }
}
