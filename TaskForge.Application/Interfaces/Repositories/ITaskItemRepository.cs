using TaskForge.Domain.Entities;
using TaskForge.Domain.Enums;

namespace TaskForge.Application.Interfaces.Repositories
{
    public interface ITaskItemRepository
    {
        Task<IEnumerable<TaskItem>> GetByProjectIdAsync(
            Guid projectId,
            TaskItemStatus? status,
            TaskItemPriority? priority,
            CancellationToken cancellationToken);

        Task<TaskItem?> GetByIdAsync(Guid projectId, Guid taskId, CancellationToken cancellationToken);
        Task AddAsync(TaskItem task, CancellationToken cancellationToken);
        Task UpdateAsync(TaskItem task, CancellationToken cancellationToken);
        Task DeleteAsync(TaskItem task, CancellationToken cancellationToken);
    }
}
