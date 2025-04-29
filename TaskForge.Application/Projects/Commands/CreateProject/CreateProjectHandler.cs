using MediatR;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Domain.Entities;

namespace TaskForge.Application.Projects.Commands.CreateProject
{
    public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, Guid>
    {
        private readonly IProjectRepository _repository;
        public CreateProjectHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateProjectCommand createProjectCommand, CancellationToken cancellationToken)
        {
            var project = new Project(Guid.NewGuid(), createProjectCommand.Name, createProjectCommand.Description);
            await _repository.AddAsync(project, cancellationToken);
            return project.Id;
        }
    }
}