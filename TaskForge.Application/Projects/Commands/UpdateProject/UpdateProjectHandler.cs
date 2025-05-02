using MediatR;
using TaskForge.Application.Interfaces.Repositories;

namespace TaskForge.Application.Projects.Commands.UpdateProject
{
    public class UpdateProjectHandler : IRequestHandler<UpdateProjectCommand, bool>
    {
        private readonly IProjectRepository _repository;
        public UpdateProjectHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
        {
            var project = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (project is null) 
                return false;

            project.UpdateDetails(command.Name, command.Description);

            await _repository.UpdateAsync(project, cancellationToken);
            return true;
        }
    }
}