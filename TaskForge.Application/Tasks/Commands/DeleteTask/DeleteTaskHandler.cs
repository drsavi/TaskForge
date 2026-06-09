using MediatR;
using TaskForge.Application.Common.Authorization;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;

namespace TaskForge.Application.Tasks.Commands.DeleteTask
{
    public class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand, bool>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskItemRepository _taskRepository;
        private readonly ICurrentUserService _currentUser;

        public DeleteTaskHandler(
            IProjectRepository projectRepository,
            ITaskItemRepository taskRepository,
            ICurrentUserService currentUser)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
        {
            await ProjectAuthorization.GetOwnedProjectAsync(
                _projectRepository,
                command.ProjectId,
                _currentUser.UserId,
                cancellationToken);

            var task = await _taskRepository.GetByIdAsync(command.ProjectId, command.TaskId, cancellationToken);

            if (task is null)
                return false;

            await _taskRepository.DeleteAsync(task, cancellationToken);
            return true;
        }
    }
}
