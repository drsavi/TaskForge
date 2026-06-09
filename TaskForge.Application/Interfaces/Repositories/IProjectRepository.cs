using TaskForge.Domain.Entities;

namespace TaskForge.Application.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdForOwnerAsync(Guid id, string ownerId, CancellationToken cancellationToken);
        Task<IEnumerable<Project>> GetAllForOwnerAsync(string ownerId, CancellationToken cancellationToken);
        Task AddAsync(Project project, CancellationToken cancellationToken);
        Task UpdateAsync(Project project, CancellationToken cancellationToken);
        Task DeleteAsync(Project project, CancellationToken cancellationToken);
    }
}
