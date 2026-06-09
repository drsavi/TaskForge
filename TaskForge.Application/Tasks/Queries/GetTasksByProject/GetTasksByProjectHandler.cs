using MediatR;
using TaskForge.Application.Common.Authorization;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Application.Mappers;

namespace TaskForge.Application.Tasks.Queries.GetTasksByProject
{
    public class GetTasksByProjectHandler : IRequestHandler<GetTasksByProjectQuery, IEnumerable<TaskItemDto>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskItemRepository _taskRepository;
        private readonly ICurrentUserService _currentUser;

        public GetTasksByProjectHandler(
            IProjectRepository projectRepository,
            ITaskItemRepository taskRepository,
            ICurrentUserService currentUser)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<TaskItemDto>> Handle(GetTasksByProjectQuery query, CancellationToken cancellationToken)
        {
            await ProjectAuthorization.GetOwnedProjectAsync(
                _projectRepository,
                query.ProjectId,
                _currentUser.UserId,
                cancellationToken);

            var tasks = await _taskRepository.GetByProjectIdAsync(
                query.ProjectId,
                query.Status,
                query.Priority,
                cancellationToken);

            return tasks.Select(t => t.ToDto());
        }
    }
}
