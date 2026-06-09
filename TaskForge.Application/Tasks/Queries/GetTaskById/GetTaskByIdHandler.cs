using MediatR;
using TaskForge.Application.Common.Authorization;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Application.Mappers;

namespace TaskForge.Application.Tasks.Queries.GetTaskById
{
    public class GetTaskByIdHandler : IRequestHandler<GetTaskByIdQuery, TaskItemDto?>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskItemRepository _taskRepository;
        private readonly ICurrentUserService _currentUser;

        public GetTaskByIdHandler(
            IProjectRepository projectRepository,
            ITaskItemRepository taskRepository,
            ICurrentUserService currentUser)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _currentUser = currentUser;
        }

        public async Task<TaskItemDto?> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
        {
            await ProjectAuthorization.GetOwnedProjectAsync(
                _projectRepository,
                query.ProjectId,
                _currentUser.UserId,
                cancellationToken);

            var task = await _taskRepository.GetByIdAsync(query.ProjectId, query.TaskId, cancellationToken);
            return task?.ToDto();
        }
    }
}
