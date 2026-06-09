using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Domain.Entities;
using TaskForge.Domain.Enums;
using TaskForge.Infrastructure.Data;

namespace TaskForge.Infrastructure.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly TaskForgeDbContext _context;
        public TaskItemRepository(TaskForgeDbContext context) => _context = context;

        public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(
            Guid projectId,
            TaskItemStatus? status,
            TaskItemPriority? priority,
            CancellationToken cancellationToken)
        {
            var query = _context.TaskItems.Where(t => t.ProjectId == projectId);

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TaskItem?> GetByIdAsync(Guid projectId, Guid taskId, CancellationToken cancellationToken)
            => await _context.TaskItems
                .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId, cancellationToken);

        public async Task AddAsync(TaskItem task, CancellationToken cancellationToken)
        {
            await _context.TaskItems.AddAsync(task, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);

        public async Task DeleteAsync(TaskItem task, CancellationToken cancellationToken)
        {
            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
