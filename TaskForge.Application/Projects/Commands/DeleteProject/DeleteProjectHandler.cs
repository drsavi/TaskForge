using MediatR;
using TaskForge.Application.Interfaces.Repositories;

namespace TaskForge.Application.Projects.Commands.DeleteProject
{
    public class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand, bool>
    {
        private readonly IProjectRepository _repository;
        public DeleteProjectHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (project is null)
                return false;

            await _repository.DeleteAsync(project, cancellationToken);
            return true;
        }
    }
}