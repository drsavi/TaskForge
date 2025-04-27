using TaskForge.Domain.Entities;

namespace TaskForge.Infrastructure.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken);
        Task AddAsync(Project project, CancellationToken cancellationToken);
        Task UpdateAsync(Project project, CancellationToken cancellationToken);
        Task DeleteAsync(Project project, CancellationToken ct);
    }
}