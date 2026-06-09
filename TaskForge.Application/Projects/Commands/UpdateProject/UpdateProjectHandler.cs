using MediatR;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;

namespace TaskForge.Application.Projects.Commands.UpdateProject
{
    public class UpdateProjectHandler : IRequestHandler<UpdateProjectCommand, bool>
    {
        private readonly IProjectRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public UpdateProjectHandler(IProjectRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
        {
            var project = await _repository.GetByIdForOwnerAsync(command.Id, _currentUser.UserId, cancellationToken);

            if (project is null)
                return false;

            project.UpdateDetails(command.Name, command.Description);
            await _repository.UpdateAsync(project, cancellationToken);
            return true;
        }
    }
}
