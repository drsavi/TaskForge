using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Domain.Entities;
using TaskForge.Infrastructure.Data;

namespace TaskForge.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly TaskForgeDbContext _context;
        public ProjectRepository(TaskForgeDbContext context) => _context = context;

        public async Task<IEnumerable<Project>> GetAllForOwnerAsync(string ownerId, CancellationToken cancellationToken)
            => await _context.Projects
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync(cancellationToken);

        public async Task<Project?> GetByIdForOwnerAsync(Guid id, string ownerId, CancellationToken cancellationToken)
            => await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId, cancellationToken);

        public async Task AddAsync(Project project, CancellationToken cancellationToken)
        {
            await _context.Projects.AddAsync(project, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Project project, CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);

        public async Task DeleteAsync(Project project, CancellationToken cancellationToken)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
