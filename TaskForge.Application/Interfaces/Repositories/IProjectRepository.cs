using TaskForge.Domain.Entities;

namespace TaskForge.Application.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken);
        Task AddAsync(Project project, CancellationToken cancellationToken);
        Task UpdateAsync(Project project, CancellationToken cancellationToken);
        Task DeleteAsync(Project project, CancellationToken cancellationToken);
    }
}