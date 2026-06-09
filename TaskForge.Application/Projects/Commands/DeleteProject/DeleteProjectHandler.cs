using MediatR;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;

namespace TaskForge.Application.Projects.Commands.DeleteProject
{
    public class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand, bool>
    {
        private readonly IProjectRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public DeleteProjectHandler(IProjectRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _repository.GetByIdForOwnerAsync(request.Id, _currentUser.UserId, cancellationToken);

            if (project is null)
                return false;

            await _repository.DeleteAsync(project, cancellationToken);
            return true;
        }
    }
}
