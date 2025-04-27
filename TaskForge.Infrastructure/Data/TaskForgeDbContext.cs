using Microsoft.EntityFrameworkCore;
using TaskForge.Domain.Entities;

namespace TaskForge.Infrastructure.Data
{
    public class TaskForgeDbContext(DbContextOptions<TaskForgeDbContext> opts) : DbContext(opts)
    {
        public DbSet<Project> Projects { get; set; } = null!;
    }
}