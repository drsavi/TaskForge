using Microsoft.EntityFrameworkCore;
using TaskForge.Domain.Entities;
using TaskForge.Infrastructure.Data;
using TaskForge.Infrastructure.Repositories.Interfaces;

namespace TaskForge.Infrastructure.Repositories.Implementations
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly TaskForgeDbContext _context;
        public ProjectRepository(TaskForgeDbContext context) => _context = context;

        public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationtoken)
            => await _context.Projects.ToListAsync(cancellationtoken);

        public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationtoken)
            => await _context.Projects.FindAsync([id], cancellationtoken);

        public async Task AddAsync(Project project, CancellationToken cancellationtoken)
        {
            await _context.Projects.AddAsync(project, cancellationtoken);
            await _context.SaveChangesAsync(cancellationtoken);
        }

        public async Task UpdateAsync(Project project, CancellationToken cancellationtoken)
            => await _context.SaveChangesAsync(cancellationtoken);

        public async Task DeleteAsync(Project project, CancellationToken cancellationtoken)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(cancellationtoken);
        }
    }
}