using MediatR;
using TaskForge.Application.Common.Authorization;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Domain.Entities;
using TaskForge.Domain.Enums;

namespace TaskForge.Application.Tasks.Commands.CreateTask
{
    public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Guid>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskItemRepository _taskRepository;
        private readonly ICurrentUserService _currentUser;

        public CreateTaskHandler(
            IProjectRepository projectRepository,
            ITaskItemRepository taskRepository,
            ICurrentUserService currentUser)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            await ProjectAuthorization.GetOwnedProjectAsync(
                _projectRepository,
                command.ProjectId,
                _currentUser.UserId,
                cancellationToken);

            var task = new TaskItem(
                Guid.NewGuid(),
                command.ProjectId,
                command.Title,
                command.Description,
                command.Priority ?? TaskItemPriority.Medium);

            await _taskRepository.AddAsync(task, cancellationToken);
            return task.Id;
        }
    }
}
